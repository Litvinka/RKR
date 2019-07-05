using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using rkrApp.Models;
using System.Data.Entity.Migrations;

namespace rkrApp.Controllers
{
    public class UsersController : Controller
    {
        private rkrDBEntities db = new rkrDBEntities();

        //Получение списка пользователей Системы
        public ActionResult Index()
        {
            if (Session["role"]==null || (Convert.ToInt32(Session["role"]) != 2 && Convert.ToInt32(Session["role"]) != 1 && Convert.ToInt32(Session["role"]) != 7)) //Данная страница доступна администраторам и региональному координатору
            {
                return Redirect("/");
            }
            List<Users> users= db.Users.Include(u => u.Roles).ToList();
            if(Convert.ToInt32(Session["role"]) == 7) //Для пользователей с ролью "Региональный координатор" отображаются только пользователи из данной области
            {
                int user=Convert.ToInt32(Session["user"]);
                int area = db.Area.First(p => p.id_coordinator == user).id;
                for(int i=0;i<users.Count;++i)
                {
                    Users u = users.ElementAt(i);
                    if (u.id_role == 1 || u.id_role == 2)
                    {
                        users.RemoveAt(i);
                        --i;
                    }
                    else if (u.id_role==3 && db.Schools.FirstOrDefault(p => p.id_user == u.id).Districts.id_area!=area)
                    {
                        users.RemoveAt(i);
                        --i;
                    }
                    else if (u.id_role == 4 && db.Schools_Observers.FirstOrDefault(p => p.id_observer == u.id).Schools.Districts.id_area!=area)
                    {
                        users.RemoveAt(i);
                        --i;
                    }
                    else if (u.id_role == 5 && db.Districts_Check.FirstOrDefault(p => p.id_user_check == u.id).Districts.id_area!=area)
                    {
                        users.RemoveAt(i);
                        --i;
                    }
                    else if (u.id_role == 6 && db.Areas_Check.FirstOrDefault(p => p.id_user_check == u.id).id_area!=area)
                    {
                        users.RemoveAt(i);
                        --i;
                    }
                    else if (u.id_role == 7 && db.Area.FirstOrDefault(p => p.id_coordinator == u.id).id != area)
                    {
                        users.RemoveAt(i);
                        --i;
                    }
                }
            }
            return View(users);
        }

        //Просмотр подробной информации о пользователе
        public ActionResult Details(int? id)
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"]) != 2 && Convert.ToInt32(Session["role"]) != 1 && Convert.ToInt32(Session["role"]) != 7))
            {
                return Redirect("/");
            }//Страница доступна для администраторов и региональных координаторов
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        //Добавление нового пользователя в Систему
        public ActionResult Create()
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"]) != 2 && Convert.ToInt32(Session["role"]) != 1))
            {
                return Redirect("/");
            } //Страница доступна для администраторов
            ViewBag.id_role = new SelectList(db.Roles, "id", "name");
            ViewBag.subjects = new SelectList(db.Subjects_Numbers, "id", "subject");
            ViewBag.areas = new SelectList(db.Area, "id", "name");
            ViewBag.areas1 = new SelectList(db.Area, "id", "name");
            return View();
        }

        //Добавление пользователя в Систему. Запись в бд
        [HttpPost]
        public ActionResult Create(Users users)
        {
            if (users!=null)
            {
                users.id = (db.Users.Count()>0) ? (db.Users.Max(p=>p.id)+1) : 1; //идентификатор для нового экземпляра класса User
                db.Users.Add(users); //добавление нового пользователя в бд
                db.SaveChanges(); //сохранение изменений в бд
                if (users.id_role == 3) //если это школьный координатор
                {
                    int id = Convert.ToInt32(Request.Form["school"]); //Идентификатор школы
                    Schools school = db.Schools.FirstOrDefault(p=>p.id==id); //Получение экземпляра класса Schools, где идентификатор равен заданному
                    school.id_user = users.id; //идентификатор пользователя
                    db.Schools.AddOrUpdate(school); //Изменение школы, назначение ей школьного координатора
                    db.SaveChanges(); //сохранение изменений
                }
                else if (users.id_role==4) //если это внешний наблюдатель, создается экземпляр класса Schools_Observers, содержащий внешних наблюдателей школ
                {
                        int school = Convert.ToInt32(Request.Form["school_watcher"]); //идентификатор школы
                        Schools_Observers so = new Schools_Observers(); //создание нового экземпляра класса Schools_Observers
                        so.id = (db.Schools_Observers.Count() > 0) ? (db.Schools_Observers.Max(d => d.id) + 1) : 1; //идентификатор для экземпляра класса Schools_Observers
                        so.id_observer = users.id; //идентификатор внешнего наблюдателя
                        so.id_school = school; //присваивание идентификатора школы
                        db.Schools_Observers.Add(so); //добавление созданного и инициализированного экземпляра в бд в таблицу Schools_Observers
                        db.SaveChanges(); //сохранение изменений в бд
                }
                else if (users.id_role == 5) //если это РПК, создается экземпляр класса Districts_Check, содержащий районных проверяющих по предметам
                {
                    int dst = Convert.ToInt32(Request.Form["district_dst"]); //идентификатор района
                    int subject = Convert.ToInt32(Request.Form["subject_dst"]); //идентификатор предмета
                    Districts_Check d = new Districts_Check(); //создание нового экземпляра класса Districts_Check
                    d.id = (db.Districts_Check.Count() > 0) ? (db.Districts_Check.Max(p => p.id) + 1) : 1; //идентификатор экземпляра класса Districts_Check
                    d.id_district = dst; //присваивание идентификатора района
                    d.id_user_check = users.id; //идентификатор районного проверяющего
                    d.id_subject_number = subject; //идентификатор предмета для проверки
                    db.Districts_Check.Add(d); //добавление в бд в таблицу Districts_Check
                    db.SaveChanges(); //сохранение
                }
                else if (users.id_role == 6) //если это ОПК, создается экземпляр класса Areas_Check, содержащий областных проверяющих по предметам
                {
                    int area = Convert.ToInt32(Request.Form["areas"]); //идентификатор области
                    int subject = Convert.ToInt32(Request.Form["subjects"]); //идентификатор предмета
                    Areas_Check a = new Areas_Check(); //создание нового экземпляра класса Areas_Check
                    a.id = (db.Areas_Check.Count() > 0) ? (db.Areas_Check.Max(p => p.id) + 1) : 1; //идентификатор для нового экземпляра класса Areas_Check
                    a.id_area = area; //присваивание идентификатора области
                    a.id_user_check = users.id; //присваивание идентификатора областного проверяющего
                    a.id_subject_number = subject; //идентификатор предмета
                    db.Areas_Check.Add(a); //добавление в бд
                    db.SaveChanges(); //сохранение
                }
                else if (users.id_role == 7) //если это региональный координатор, добавляем его в бд к области
                {
                    int area = Convert.ToInt32(Request.Form["areas1"]); //идентификатор области, региональным координатором которой будет данный пользователь
                    Area a = db.Area.FirstOrDefault(p=>p.id==area); //получение экземпляра класса Area по идентификатору
                    a.id_coordinator = users.id; //идентификатор регионального координатора
                    db.Area.AddOrUpdate(a); //изменение экземпляра Area в бд
                    db.SaveChanges(); //сохранение
                }
                return RedirectToAction("Index");
            }
            ViewBag.id_role = new SelectList(db.Roles, "id", "name", users.id_role);
            return View(users);
        }

        //Редактирование информации о пользователе, доступно администраторам и региональному координатору
        public ActionResult Edit(int? id)
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"]) != 2 && Convert.ToInt32(Session["role"]) != 1 && Convert.ToInt32(Session["role"]) != 7))
            {
                return Redirect("/");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_role = new SelectList(db.Roles, "id", "name", users.id_role);
            ViewBag.subjects = new SelectList(db.Subjects_Numbers, "id", "subject", users.getSubject());
            if (users.id_role == 6)
            {
                ViewBag.areas = new SelectList(db.Area, "id", "name", users.Areas_Check.FirstOrDefault(p=>p.id_user_check==users.id).id_area);
            }
            else
            {
                ViewBag.areas = new SelectList(db.Area, "id", "name");
            }
            if(users.id_role == 7)
            {
                ViewBag.areas1 = new SelectList(db.Area, "id", "name", users.Area.FirstOrDefault(p=>p.id_coordinator==users.id).id);
            }
            else
            {
                ViewBag.areas1 = new SelectList(db.Area, "id", "name");
            }
            return View(users);
        }


        //Редактирование основной информации о пользователе (ФИО и email)
        [HttpPost]
        public ActionResult Edit(Users users)
        {
            
            if (ModelState.IsValid)
            {
                int count = db.Users.Count(p => p.email.Equals(users.email.Trim()) && p.id != users.id);
                if (count == 0)
                {
                    db.Users.AddOrUpdate(users);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else if (count > 0)
                {
                    users.email = db.Users.Find(users.id).email;
                    ViewBag.error = "Такой email уже есть в базе данных";
                }
            }
            users.Roles=db.Roles.Find(users.id_role);
            return View(users);
        }


        //Удаление пользователя из базы данных. Доступно только администраторам
        public ActionResult Delete(int? id)
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"]) != 2 && Convert.ToInt32(Session["role"]) != 1))
            {
                return Redirect("/");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            if (users.id_role == 3)
            {
                Schools s = db.Schools.FirstOrDefault(p => p.id_user == users.id);
                s.id_user = null;
                db.Schools.AddOrUpdate(s);
                db.SaveChanges();
            }
            else if (users.id_role == 4)
            {
                Schools_Observers x = db.Schools_Observers.FirstOrDefault(p=>p.id_observer==id);
                db.Schools_Observers.Remove(x);
                db.SaveChanges();

            }
            else if (users.id_role == 5)
            {
                Districts_Check d = db.Districts_Check.FirstOrDefault(p => p.id_user_check == users.id);
                db.Districts_Check.Remove(d);
                db.SaveChanges();
            }
            else if (users.id_role == 6)
            {
                Areas_Check a = db.Areas_Check.FirstOrDefault(p => p.id_user_check == users.id);
                db.Areas_Check.Remove(a);
                db.SaveChanges();
            }
            else if (users.id_role == 7)
            {
                Area a = db.Area.FirstOrDefault(p => p.id_coordinator == users.id);
                a.id_coordinator = null;
                db.Area.AddOrUpdate(a);
                db.SaveChanges();
            }
            db.Users.Remove(users);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //Сброс пароля у пользователей. Доступно администраторам и региональным координаторам
        public ActionResult DeletePassword(int? id)
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"]) != 2 && Convert.ToInt32(Session["role"]) != 1 && Convert.ToInt32(Session["role"]) != 7))
            {
                return Redirect("/");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            users.password = null;
            db.Users.AddOrUpdate(users);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

            public ActionResult _Zavuch(int? user_id)
        {
            List<SelectListItem> s = new List<SelectListItem>();
            foreach (var a in db.Subjects_Numbers)
            {
                String g = a.number + " (" + a.subject + ")";
                s.Add(new SelectListItem { Text = g, Value = a.id.ToString() });
            }
            if (user_id == null)
            {
                ViewBag.id_area = new SelectList(db.Area, "id", "name");
                ViewBag.id_district = new SelectList(db.Districts, "id", "name");
                ViewBag.school = new SelectList(db.Schools, "id", "name");
                ViewBag.subject = new SelectList(s, "Value", "Text");
                return PartialView();
            }
            Schools school = db.Schools.FirstOrDefault(p=>p.id_user==user_id);
            if (school == null)
            {
                ViewBag.id_area = new SelectList(db.Area, "id", "name");
                ViewBag.id_district = new SelectList(db.Districts, "id", "name");
                ViewBag.school = new SelectList(db.Schools, "id", "name");
                ViewBag.subject = new SelectList(s, "Value", "Text");
                return PartialView();
            }
            ViewBag.id_area = new SelectList(db.Area, "id", "name", school.Districts.id_area);
            var dstr = db.Districts.Where(p => p.id_area == school.Districts.id_area);
            ViewBag.id_district = new SelectList(dstr, "id", "name", school.id_district);
            var scl = db.Schools.Where(p=>p.id_district==school.id_district);
            ViewBag.school = new SelectList(scl, "id", "name", school.id);
            ViewBag.subject = new SelectList(s, "Value", "Text", school.Classes.FirstOrDefault(p=>p.id_school==school.id).id_subject_number);
            return PartialView(school);
        }

        public ActionResult _Watcher(int? user_id)
        {
            List<SelectListItem> s = new List<SelectListItem>();
            foreach (var a in db.Subjects_Numbers)
            {
                String g = a.number + " (" + a.subject + ")";
                s.Add(new SelectListItem { Text = g, Value = a.id.ToString() });
            }
            if (user_id == null)
            {
                ViewBag.area_watcher = new SelectList(db.Area, "id", "name");
                ViewBag.district_watcher = new SelectList(db.Districts, "id", "name");
                ViewBag.school_watcher = new SelectList(db.Schools, "id", "name");
                ViewBag.subject_watcher = new SelectList(s, "Value", "Text");
                return PartialView();
            }
            Classes cl = db.Classes.FirstOrDefault(p=>p.Schools.Schools_Observers.FirstOrDefault(d=>d.id_observer==user_id)!=null);
            if (cl == null){
                ViewBag.area_watcher = new SelectList(db.Area, "id", "name");
                ViewBag.district_watcher = new SelectList(db.Districts, "id", "name");
                ViewBag.school_watcher = new SelectList(db.Schools, "id", "name");
                ViewBag.subject_watcher = new SelectList(s, "Value", "Text");
                return PartialView();
            }
            Schools school = db.Schools.Find(cl.id_school);
            ViewBag.area_watcher = new SelectList(db.Area, "id", "name", school.Districts.id_area);
            var dst = db.Districts.Where(p => p.id_area == school.Districts.id_area);
            ViewBag.district_watcher = new SelectList(dst, "id", "name", school.id_district);
            var scl = db.Schools.Where(p => p.id_district == school.id_district);
            ViewBag.school_watcher = new SelectList(scl, "id", "name", school.id);
            var wat = db.Classes.Where(p => p.id_school == school.id);
            ViewBag.subject_watcher = new SelectList(s, "Value", "Text", school.Classes.FirstOrDefault(p => p.id_school == school.id).id_subject_number);

            var res = db.Classes.Where(p=>p.Schools.Schools_Observers.FirstOrDefault(d=>d.id_observer==user_id)!=null && p.letter!=null);
            String let = "";
            int i = 0;
            foreach(var item in res)
            {
                let += item.letter;
                if (i < res.Count() - 1)
                {
                    let += ", ";
                }
                ++i;
            }
            ViewBag.letters = let;

            return PartialView(school);
        }


        public ActionResult _District(int? user_id)
        {
            if (user_id == null)
            {
                ViewBag.area_dst = new SelectList(db.Area, "id", "name");
                ViewBag.district_dst = new SelectList(db.Districts, "id", "name");
                ViewBag.subject_dst = new SelectList(db.Subjects_Numbers, "id", "subject");
                return PartialView();
            }
            else
            {
                Districts_Check ds = db.Districts_Check.FirstOrDefault(p=>p.id_user_check==user_id);
                if (ds == null)
                {
                    ViewBag.area_dst = new SelectList(db.Area, "id", "name");
                    ViewBag.district_dst = new SelectList(db.Districts, "id", "name");
                    ViewBag.subject_dst = new SelectList(db.Subjects_Numbers, "id", "subject");
                    return PartialView();
                }
                ViewBag.area_dst = new SelectList(db.Area, "id", "name", ds.Districts.id_area);
                var dst = db.Districts.Where(p => p.id_area == ds.Districts.id_area);
                ViewBag.district_dst = new SelectList(db.Districts, "id", "name", ds.id_district);
                ViewBag.subject_dst = new SelectList(db.Subjects_Numbers, "id", "subject", ds.id_subject_number);
                return PartialView(ds);
            }

        }

        //Возвращает список районной выбранной области, преобразованный в .json
        [HttpPost]
        public JsonResult AllDistrict(string param)
        {
            int s = Convert.ToInt32(param); //Получение параметра "идентификатор области"
            var local = db.Districts.Where(p => p.id_area == s); //Все районы заданной области
            List<List<String>> list = new List<List<String>>(); //Новый список для хранения значений (идентификатор района из бд, название района)
            List<String> row;
            foreach (Districts p in local)
            {
                row = new List<String>();
                row.Add(Convert.ToString(p.id));
                row.Add(p.name);
                list.Add(row);
            } //Запись информации о районе в массив
            var d = System.Web.Helpers.Json.Encode(list); //преобразование списка, содержащего необходимую информацию о районах, в формат .json
            return Json(d);
        }

        
        //[HttpPost]
        //public JsonResult AllLetter(string param, string param2)
        //{
        //    int s = Convert.ToInt32(param);
        //    int k = Convert.ToInt32(param2);
        //    var local = db.Classes.Where(p => p.id_school == k && p.id_subject_number==s && p.letter!=null && p.id_observer==null);
        //    List<List<String>> list = new List<List<String>>();
        //    List<String> row;
        //    foreach (Classes p in local)
        //    {
        //        row = new List<String>();
        //        row.Add(Convert.ToString(p.id));
        //        row.Add(p.letter);
        //        list.Add(row);
        //    }
        //    var d = System.Web.Helpers.Json.Encode(list);
        //    return Json(d);
        //}


        [HttpPost]
        public JsonResult AllSchool(string param, string role)
        {
            int s = Convert.ToInt32(param); //идентификатор района
            int r = Convert.ToInt32(role); //идентификатор роль пользователя
            List<Schools> local = db.Schools.Where(p => p.id_district == s).ToList(); //список школ, расположенных в заданном районе
            if (r == 3)
            {
                local = local.Where(p=>p.id_user==null).ToList(); //список школ, у которых нет школьных координаторов
            }
            List<List<String>> list = new List<List<String>>();
            List<String> row;
            foreach (Schools p in local)
            {
                row = new List<String>();
                row.Add(Convert.ToString(p.id));
                row.Add(p.name);
                list.Add(row);
            } //запись данных о школе в список (идентификатор школы, наименование школы)
            var d = System.Web.Helpers.Json.Encode(list); //преобразование списка, содержащего необходимую информацию о школах, в формат .json
            return Json(d);
        }


        //Распределение работ для районной проверки
        public ActionResult Distribution()
        {
            ViewBag.subject_id = new SelectList(db.Subjects_Numbers, "id", "subject");
            return View();
        }

        //Распределение работ для районной проверки. Запись в бд результатов распределения
        [HttpPost]
        public ActionResult DistributionDistrict(int subject_id)
        {
            List<Area> area = db.Area.ToList(); //Список областей
            List<Subjects_Numbers> subjects = db.Subjects_Numbers.ToList(); //Список предметов
            List<Results> results = new List<Results>(); //Список для хранения результатов распределения работ для районной проверки
            List<Students> students = db.Students.Where(p => p.Classes.id_subject_number== subject_id && p.cipher != null && p.scan != null).ToList(); //список учащихся, пишущих РКР по заданному предмету и у которых добавлены шифр и бланк результатов
            List<int> area_count_max = new List<int>(); //Список, содержащий максимальное количество работ, отправленных на районную проверку в области
            List<int> area_count_now = new List<int>(); //Список, содержащий количество работ, на данных момент отправленных на районную проверку в области
            List<Students>[] res_area = new List<Students>[area.Count()]; //Массив списков, содержащий распределение учащихся по областям для районной проверки
            int index = 0;
            for (int i = 0; i < area.Count(); ++i) //Добавление данных в списки
            {
                area_count_max.Add(students.Count(p => p.Classes.Schools.Districts.id_area == area[i].id));
                area_count_now.Add(0);
                res_area[i] = new List<Students>();
            }

            for (int i = 0; i < area.Count()-1; ++i)
            {
                for(int j=0;j< area.Count()-1-i; ++j)
                {
                    if (area_count_max[j] < area_count_max[j+1])
                    {
                        Area z = area[j];
                        area[j] = area.ElementAt(j+1);
                        area[j+1] = z;
                        int t = area_count_max[j];
                        area_count_max[j] = area_count_max[j+1];
                        area_count_max[j+1] = t;
                    }
                }
            }

            int a = 0; //номер области, в которую будет отправляться работа на проверку
            for (int i = 0; i < area.Count(); ++i) //перебор областей
                {
                    List<Students> std = students.Where(p => p.Classes.Schools.Districts.id_area == area[i].id).ToList(); //Список учащихся из данной области
                    for (int j = 0; j < std.Count(); ++j) //перебор учащихся
                    {
                        if (i == a) { a = (a == (area.Count() - 1)) ? 0 : (a + 1); } //если номер области для отправки работы на проверку и номер текущей области совпадает, то номер области для проверки увеличивается на 1 (если меньше количества областей - 1) или принимает значение 0

                        if ((i == (area.Count() - 2)) && (area_count_now[area.Count() - 1] < area_count_max[area.Count() - 1]))
                        {
                            res_area[area.Count() - 1].Add(std[j]);
                            area_count_now[area.Count() - 1]++;
                            a = (a == (area.Count() - 1)) ? 0 : (a + 1);
                            continue;
                        }

                        if (area_count_now[a] < area_count_max[a]) //если в данную область еще можно добавлять работы на проверку, то добавляем работу
                        {
                            res_area[a].Add(std[j]);
                            area_count_now[a]++;
                        }
                        else 
                        {
                            --j;
                        }
                        a = (a == (area.Count() - 1)) ? 0 : (a + 1); //номер области для проверки увеличивается на 1 (если меньше количества областей - 1) или принимает значение 0
                }
                } //Разделение по областям


                //Распределение по районам
                List<Districts> districts = db.Districts.Where(p=>p.Districts_Check.FirstOrDefault(s=>s.id_subject_number==subject_id)!=null).ToList(); //Список всех районов
                int ind = (db.Results.Count() > 0) ? (db.Results.Max(p => p.id) + 1) : 1; //идентификатор, с которого нужно добавлять результаты в бд в таблицу Results
                for (int i = 0; i < area.Count(); ++i)
                {
                    int area_index = 0;
                    List<Districts> ds = districts.Where(p => p.id_area == area[i].id).ToList(); //Список районов данной области
                    for (int j = 0; j < ds.Count(); ++j)
                    {
                        int district_id = ds.ElementAt(j).id;
                        int c_d = students.Count(p => p.Classes.Schools.id_district == district_id);
                        for (int f = 0; f < c_d; ++f)
                        {
                                Results r = new Results();
                                r.number_verification = 1;
                            r.id = ind++;
                            r.id_student = res_area[i][area_index++].id;
                            r.id_user = db.Districts_Check.First(p => p.id_district == district_id && p.id_subject_number == subject_id).id_user_check;
                            results.Add(r);
                        }
                    }
                }
                db.Results.AddRange(results); //добавление результатов
                db.SaveChanges();
                //Распределение по районам

            return Redirect("/Users/Index");
        }

        //Распределение работ по областям для проверки
        public ActionResult DistributionArea()
        {
            ViewBag.subject_id = new SelectList(db.Subjects_Numbers, "id", "subject");
            return View();
        }


        //Распределение третьей части работ для проверки ОПК. Запись в бд
        [HttpPost]
        public ActionResult DistributionArea(int subject_id)
        {
            List<Area> a = db.Area.ToList(); //Список областей
            List<Areas_Check> s_c = db.Areas_Check.Where(p=>p.id_subject_number==subject_id).ToList(); //Список проверяющих ОПК
            List<Results> results = new List<Results>(); //Список для добавления результатов распределения
            int id = db.Results.Max(p => p.id) + 1; //Идентификатор для результатов, с которого будет начата запись в бд
            for(int k=0;k<a.Count();++k) //перебор областей
            {
                int area_id = a.ElementAt(k).id; //получение идентификатора области
                List<Results> r = db.Results.Where(p => p.number_verification == 1 && p.Students.Classes.id_subject_number==subject_id && p.Users.Districts_Check.FirstOrDefault().Districts.id_area == area_id).ToList(); //список результатов первой проверки работ, которые на районной проверке проверяли комиссии из данной области
                for(int i = 0; i < r.Count(); i += 3) //перебор результатов и добавление каждого третьего на областную проверку в данную область
                {
                    Results t = new Results(); //создание нового экземпляра класса Results
                    t.id = id++; //присваивание идентификатора
                    t.id_student = r.ElementAt(i).id_student; //присваивание идентификатора учащегося, работу которого будут проверять
                    t.number_verification = 3; //номер проверки
                    t.id_user = s_c.FirstOrDefault(p=>p.id_area==area_id).id_user_check; //идентификатор пользователя, который будет проверять данную работу
                    results.Add(t); //добавление экземпляра в список
                }
            }
            db.Results.AddRange(results); //добавление списка результатов в бд
            db.SaveChanges(); //сохранение результатов
            return Redirect("/Users/Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
