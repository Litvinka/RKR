using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using rkrApp.Models;
using System.Data.Entity.Migrations;

namespace rkrApp.Controllers
{
    public class HomeController : Controller
    {
        private rkrDBEntities db = new rkrDBEntities();

        //После успешного входа в Систему. Перенаправление на главную страницу в зависимости от роли
        public ActionResult Index()
        {
            if (Session["role"] != null)
            {
                int role_id = Convert.ToInt32(Session["role"]);
                int user_id = Convert.ToInt32(Session["user"]);

                //Sessions ss = new Sessions();
                //ss.id = (db.Sessions.Count() > 0) ? db.Sessions.Max(p => p.id) + 1 : 1;
                //ss.id_user = user_id;
                //ss.date_enter = DateTime.Now;
                //db.Sessions.Add(ss);
                //db.SaveChanges();
                if (role_id == 5 || role_id == 6 || role_id == 8)
                {
                    return Redirect("/Results/Works");
                }
                else if (role_id == 1|| role_id == 2)
                {
                    return Redirect("/Users/Index");
                }
                //Региональный координатор
                else if (role_id == 7)
                {
                    return Redirect("/RegionCoordinator/Index");
                }
                else if (role_id == 3)
                {
                    if (db.Schools.Where(s => s.id_user == user_id).First().id_place != null && db.Schools.Where(s => s.id_user == user_id).First().id_type_edy != null)
                        return Redirect("/Coordinator/IndexClasses/");
                    else

                        return Redirect("/Coordinator/EditSchool/");
                }
                //Наблюдатель
                else if (role_id == 4)
                {
                    return Redirect("/Observer/AddCipher");
                }
            }
            return Redirect("/Home/Login");
        }

        //Страница входа в Систему
        public ActionResult Login(Users s)
        {
            if (s == null)
            {
                s = new Users();
            }
            return View(s);
        }


        //Получение роли пользователя и преобразование в .json
        [HttpPost]
        public JsonResult GetUserRole()
        {
            int role = (Session["role"] != null) ? Convert.ToInt32(Session["role"]) : 0;
            var d = System.Web.Helpers.Json.Encode(role);
            return Json(role);
        }

        //Получение предмета по роли и идентификатору пользователя
        [HttpPost]
        public JsonResult GetUserSubject()
        {
            int role = Convert.ToInt32(Session["role"]);
            int user= Convert.ToInt32(Session["user"]);
            int subject = 0;
            if (role == 4)
            {
                subject = db.Schools_Observers.First(p => p.id_observer == user).Schools.Classes.First().id_subject_number;
            }
            else if (role == 5)
            {
                subject = db.Districts_Check.First(p => p.id_user_check == user).id_subject_number;
            }
            else if (role == 6)
            {
                subject = db.Areas_Check.First(p => p.id_user_check == user).id_subject_number;
            }
            return Json(subject);
        }

        //Страница, на которую происходит перенаправление, если для пользователя на данный момент закрыт доступ к Системе
        public ActionResult NotAccess()
        {
            List<Time_X> times = db.Time_X.OrderBy(p => p.start).ToList();
            return View(times);
        }

        //Вход пользователя в Систему. Аутентификация и авторизация
        [HttpPost]
        public ActionResult Login(String email, String pass)
        {
            if (Session["role"] != null)
            {
                return Redirect("/");
            }
                Users user = db.Users.FirstOrDefault(p => p.email.Equals(email) && p.password.Equals(pass));
            if (user != null)
            {
                if(user.id_role<=3 || user.id_role == 7)
                {
                    Session["role"] = user.id_role;
                    Session["user"] = user.id;
                    return Redirect("/");
                }
                int subject = 0;
                List<Time_X> time = db.Time_X.ToList();
                if (user.id_role == 4)
                {
                    //Session["role"] = user.id_role;
                    //Session["user"] = user.id;
                    //return Redirect("/");
                    subject = (db.Schools.First(s => s.Schools_Observers.Count(p => p.id_observer == user.id) > 0).Classes.FirstOrDefault() != null) ? db.Schools.First(s => s.Schools_Observers.Count(p => p.id_observer == user.id) > 0).Classes.FirstOrDefault().id_subject_number : 0;
                }
                else if (user.id_role == 5)
                {
                    subject = db.Districts_Check.FirstOrDefault(p => p.id_user_check == user.id).Subjects_Numbers.id;
                }
                else if (user.id_role == 6)
                {
                    subject = db.Areas_Check.FirstOrDefault(p => p.id_user_check == user.id).Subjects_Numbers.id;
                }
                Time_X t = time.FirstOrDefault(p => p.id_role == user.id_role && p.id_subject_number == subject && p.start <= DateTime.Now);
                if (t!=null)
                {
                    Session["role"] = user.id_role;
                    Session["user"] = user.id;
                    return Redirect("/");
                }
                else
                {
                    return Redirect("/Home/NotAccess");
                }
            }
            else
            {
                ViewBag.err = "Email или пароль неверные";
                user = new Users();
                user.email = email;
                user.password = pass;
            }
            return View(user);
        }

        //Вход пользователя в Систему в первый раз
        public ActionResult Enter(int step = 1)
        {
            ViewBag.step = step;
            if (Session["email"] != null)
            {
                String email = Session["email"].ToString();
                ViewBag.email = email;
                ViewBag.role_name = db.Users.FirstOrDefault(p => p.email.Equals(email)).Roles.name;
                ViewBag.role = db.Users.FirstOrDefault(p => p.email.Equals(email)).id_role;
            }
            return View();
        }

        //Меню для школьных координаторов
        public ActionResult _SchoolNav()
        {
            int user_id = Convert.ToInt32(Session["user"]);
            Schools school = db.Schools.FirstOrDefault(p=>p.id_user==user_id);
            return PartialView(school);
        }

        //Меню для районных проверяющих
        public ActionResult _DistrictNav()
        {
            int user_id = Convert.ToInt32(Session["user"]);
            Districts_Check dst = db.Districts_Check.FirstOrDefault(p => p.id_user_check == user_id);
            return PartialView(dst);
        }

        //Меню для внешних наблюдателей
        public ActionResult _WatcherNav()
        {
            int user_id = Convert.ToInt32(Session["user"]);
            return PartialView(db.Schools_Observers.FirstOrDefault(p=>p.id_observer==user_id));
        }

        //Меню для областных проверяющих
        public ActionResult _AreaNav()
        {
            int user_id = Convert.ToInt32(Session["user"]);
            int role_id = Convert.ToInt32(Session["role"]);
            Areas_Check area = new Areas_Check();
            area = db.Areas_Check.FirstOrDefault(p => p.id_user_check == user_id);
            return PartialView(area);
        }

        //Меню для региональных координаторов
        public ActionResult _AreaCoordinatorNav()
        {
            int user_id = Convert.ToInt32(Session["user"]);
            Area area = db.Area.FirstOrDefault(p => p.id_coordinator == user_id);
            return PartialView(area);
        }

        //Выход из Системы
        public ActionResult OutSite()
        {
            //int user_id = Convert.ToInt32(Session["user"]);
            //Sessions ss = db.Sessions.OrderByDescending(p=>p.date_enter).FirstOrDefault(p=>p.id_user==user_id);
            //ss.date_exit = DateTime.Now;
            //db.Sessions.AddOrUpdate(ss);
            //db.SaveChanges();

            Session["role"] = null;
            Session["user"] = null;
            return Redirect("/");
        }


        //Вход в Систему
        [HttpPost]
        public ActionResult Enter(int step, String email)
        {
            if (step == 1)
            {
                Users user = db.Users.FirstOrDefault(p => p.email.Equals(email));
                ViewBag.email = email;
                if (user != null)
                {
                    if (user.password != null)
                    {
                        return Redirect("/Home/Login");
                    }
                    Session["email"] = user.email;
                    step = 2;
                }
                else
                {
                    step = 3;
                }
            }
            else if (step == 2)
            {
                String pass = Request.Form["pass"];
                Users u = db.Users.FirstOrDefault(p => p.email == email);
                u.password = pass;
                db.Users.AddOrUpdate(u);
                db.SaveChanges();
                Session["email"] = null;
                return Redirect("/");
            }
            return RedirectToAction("Enter", "Home", new { step = step });
        }

    }
}