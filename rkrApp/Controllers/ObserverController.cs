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
using System.Threading.Tasks;

namespace rkrApp.Controllers
{
    public class ObserverController : Controller
    {
        private rkrDBEntities db = new rkrDBEntities();

        //Список учащихся и их шифров
        public async Task<ActionResult> IndexCiphers()
        {
            int user_id = Convert.ToInt32(Session["user"]);
            List<Students> std = await db.Students.Where(p=>p.Classes.Schools.Schools_Observers.FirstOrDefault(d=>d.id_observer==user_id)!=null).ToListAsync();
            ViewBag.add = std.Where(p => p.cipher != null).Count();
            int all = std.Count();
            ViewBag.all = all;
            if (all > 0)
            {
                int subject = std.First(p => p.Classes.Schools.Schools_Observers.FirstOrDefault(d => d.id_observer == user_id) != null).Classes.id_subject_number;
                Time_X t = await db.Time_X.FirstOrDefaultAsync(p => p.id_role == 4 && p.id_subject_number == subject);
                ViewBag.start = t.start;
                ViewBag.finish = t.finish;
            }
            std = std.Where(p => p.cipher != null).ToList();
            return View(std);
        }


        //Список учащихся и их бланков ответов
        public ActionResult _IndexScans()
        {
            int user_id = Convert.ToInt32(Session["user"]);
            List<Students> students = db.Students.Where(p=>p.Classes.Schools.Schools_Observers.FirstOrDefault(d => d.id_observer == user_id) != null).ToList();
            ViewBag.upload = students.Count(p=>p.scan!=null);
            ViewBag.all = students.Count();
            students = students.Where(p=>p.scan != null).ToList();
            if (students.Count() > 0)
            {
                int subject = students.FirstOrDefault().Classes.id_subject_number;
                Time_X t = db.Time_X.FirstOrDefault(p => p.id_role == 4 && p.id_subject_number == subject);
                ViewBag.start = t.start;
                ViewBag.finish = t.finish;
            }
            return PartialView(students);
        }

        // GET: Observer/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"]) != 4))
            {
                return Redirect("/");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Students students = db.Students.Find(id);
            if (students == null)
            {
                return HttpNotFound();
            }
            return View(students);
        }

        //Добавление шифра к работе
        public async Task<ActionResult> AddCipher()
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"]) != 4))
            {
                return Redirect("/");
            }
            int user_id = Convert.ToInt32(Session["user"]);
            List<Classes> cl = await db.Classes.Where(p => p.Schools.Schools_Observers.FirstOrDefault(d=>d.id_observer == user_id)!=null).ToListAsync();
            ViewBag.letters = (cl.FirstOrDefault(p=>p.letter!=null) !=null) ? (new SelectList(cl, "id", "letter")) : null;
            Schools_Observers s_o = await db.Schools_Observers.FirstAsync(p => p.id_observer == user_id);
            ViewBag.school_code = s_o.Schools.code;
            ViewBag.date = (cl.Count()!=0 && cl.First().Students.FirstOrDefault() != null) ? cl.First().Students.First().date : "27";
            List<Students> std = await db.Students.Where(p => p.Classes.Schools.Schools_Observers.FirstOrDefault(d => d.id_observer == user_id) != null).ToListAsync();
            ViewBag.students = new SelectList(std.OrderBy(p => p.number_in_the_list).ToList(), "id", "allname");
            ViewBag.add =std.Where(p => p.cipher != null).Count();
            int all=std.Count();
            ViewBag.all = all;
            if (all > 0)
            {
                int subject = std.First(p => p.Classes.Schools.Schools_Observers.FirstOrDefault(d => d.id_observer == user_id) != null).Classes.id_subject_number;
                Time_X t = await db.Time_X.FirstOrDefaultAsync(p => p.id_role == 4 && p.id_subject_number == subject);
                ViewBag.start = t.start;
                ViewBag.finish = t.finish;
            }
            return View();
        }


        public async Task<ActionResult> Works()
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"]) != 4))
            {
                return Redirect("/");
            }
            int user_id = Convert.ToInt32(Session["user"]);
            Schools school = await db.Schools.FirstAsync(p => p.Schools_Observers.Count(s => s.id_observer == user_id) > 0);
            int subject = (school.Classes.FirstOrDefault()!=null) ? school.Classes.FirstOrDefault().id_subject_number : 0;
            List<Documents> d = await db.Documents.Where(p => p.id_type_document == 1 && p.id_subject_number==subject).ToListAsync();
            return View(d);
        }


        //Изменение шифра работы. Страница
        public async Task<ActionResult> UpdateCipher(int? student_id)
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"]) != 4))
            {
                return Redirect("/");
            }
            if (student_id == null){
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Students std = await db.Students.FindAsync(student_id);
            if (std == null) {
                return HttpNotFound();
            }
            return View(std);
        }

        //Изменение шифра работы. Запись в бд
        [HttpPost]
        public async Task<ActionResult> UpdateCipher(int id, string cipher)
        {
            Students std = await db.Students.FindAsync(id);
            cipher = std.date;
            cipher += std.Classes.Schools.code + Request.Form["number"] + Request.Form["variant"];
            std.cipher = cipher;
            std.scan = null;
            std.last_change = DateTime.Now;
            db.Students.AddOrUpdate(std);
            await db.SaveChangesAsync();
            return RedirectToAction("AddCipher");
        }

        //Возвращаются данные в формате .json об учащихся (идентификатор, полное имя), у которых не задан шифр
        [HttpPost]
        public JsonResult GetStudentClass(string param)
        {
            int s = Convert.ToInt32(param);
            var std = db.Students.Where(p => p.id_classs == s && p.cipher == null);
            List<List<String>> list = new List<List<String>>();
            List<String> row;
            foreach (Students p in std)
            {
                row = new List<String>();
                row.Add(Convert.ToString(p.id));
                row.Add(p.allname);
                list.Add(row);
            }
            var d = System.Web.Helpers.Json.Encode(list);
            return Json(d);
        }

        //Добавление шифра для работы учащегося. Запись в бд
        [HttpPost]
        public async Task<ActionResult> AddCipher(int students, String cipher)
        {
            Students student = await db.Students.FindAsync(students);
            cipher = student.date;
            cipher += student.Classes.Schools.code + Request.Form["number"] + Request.Form["variant"];
            student.cipher = cipher;
            student.last_change = DateTime.Now;
            db.Students.AddOrUpdate(student);
            await db.SaveChangesAsync();
            ViewBag.added = "Шифр добавлен";

            int user_id = Convert.ToInt32(Session["user"]);
            List<Classes> cl = await db.Classes.Where(p => p.Schools.Schools_Observers.FirstOrDefault(d => d.id_observer == user_id) != null).ToListAsync();
            ViewBag.letters = (cl.FirstOrDefault(p => p.letter != null) != null) ? (new SelectList(cl, "id", "letter")) : null;
            Schools_Observers s_o= await db.Schools_Observers.FirstAsync(p => p.id_observer == user_id);
            ViewBag.school_code=s_o.Schools.code;
            ViewBag.date = (cl.Count() != 0 && cl.First().Students.FirstOrDefault() != null) ? cl.First().Students.First().date : "27";
            List<Students> std = await db.Students.Where(p => p.Classes.Schools.Schools_Observers.FirstOrDefault(d => d.id_observer == user_id) != null).ToListAsync();
            ViewBag.students = new SelectList(std.OrderBy(p => p.number_in_the_list).ToList(), "id", "allname");
            ViewBag.add = std.Where(p => p.cipher != null).Count();
            int all = std.Count();
            ViewBag.all = all;
            if (all > 0)
            {
                int subject = std.First(p => p.Classes.Schools.Schools_Observers.FirstOrDefault(d => d.id_observer == user_id) != null).Classes.id_subject_number;
                Time_X t = await db.Time_X.FirstOrDefaultAsync(p => p.id_role == 4 && p.id_subject_number == subject);
                ViewBag.start = t.start;
                ViewBag.finish = t.finish;
            }
            return View();
        }

        //Страница, где добавляются бланки ответов
        public async Task<ActionResult> AddScan()
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"]) != 4))
            {
                return Redirect("/");
            }
            ViewBag.id_classs = new SelectList(db.Classes, "id", "letter");
            ViewBag.id_gender = new SelectList(db.Genders, "id", "name");

            int user_id = Convert.ToInt32(Session["user"]);
            List<Students> std = await db.Students.Where(p => p.Classes.Schools.Schools_Observers.FirstOrDefault(d => d.id_observer == user_id) != null).ToListAsync();
            ViewBag.add = std.Where(p => p.cipher != null).Count();
            int all = std.Count();
            ViewBag.all = all;
            if (all > 0)
            {
                int subject = std.First().Classes.id_subject_number;
                Time_X t = await db.Time_X.FirstOrDefaultAsync(p => p.id_role == 4 && p.id_subject_number == subject);
                ViewBag.start = t.start;
                ViewBag.finish = t.finish;
            }
            return View();
        }


        //Добавление бланков ответов в Систему. Сохранение в бд
        [HttpPost]
        public async Task<ActionResult> AddScan(IEnumerable<HttpPostedFileBase> uploads, Students students)
        {
            foreach (var file in uploads)
            {
                if (file != null)
                {
                    int ext_length = System.IO.Path.GetExtension(file.FileName).Length;
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    string exstension = System.IO.Path.GetExtension(file.FileName).ToLower();
                    string name = fileName.Substring(0, fileName.Length - ext_length);
                    Students std = await db.Students.FirstOrDefaultAsync(p => p.cipher == name);
                    if (std != null && (exstension == ".jpeg" || exstension == ".jpg"))
                    {
                        file.SaveAs(Server.MapPath("~/Content/blank_results/" + fileName));
                        std.scan = "/Content/blank_results/" + fileName;
                        std.last_change = DateTime.Now;
                        db.Students.AddOrUpdate(std);
                        await db.SaveChangesAsync();
                    }
                }
            }
            ViewBag.id_classs = new SelectList(db.Classes, "id", "letter", students.id_classs);
            ViewBag.id_gender = new SelectList(db.Genders, "id", "name", students.id_gender);
            return RedirectToAction("AddScan");
        }

        //Страница, на которой можно изменить бланк ответов
        public async Task<ActionResult> EditBlank(int? id)
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"]) != 4))
            {
                return Redirect("/");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Students students = await db.Students.FindAsync(id);
            if (students == null)
            {
                return HttpNotFound();
            }
            return View(students);
        }

        //Изменение бланков ответов и сохранение новых в папке на сервере
        [HttpPost]
        public async Task<ActionResult> EditBlank(HttpPostedFileBase file, Students students)
        {
            if (file != null)
            {
                int ext_length = System.IO.Path.GetExtension(file.FileName).Length;
                string fileName = System.IO.Path.GetFileName(file.FileName);
                string exstension= System.IO.Path.GetExtension(file.FileName).ToLower();
                string name = fileName.Substring(0, fileName.Length - ext_length);
                Students std = await db.Students.FindAsync(students.id);
                if (std.cipher == name && (exstension==".jpeg" || exstension == ".jpg"))
                {
                    file.SaveAs(Server.MapPath("~/Content/blank_results/" + fileName));
                    std.scan = "/Content/blank_results/" + fileName;
                    std.last_change = DateTime.Now;
                    db.Students.AddOrUpdate(std);
                    await db.SaveChangesAsync();
                    List<Results> listResults = students.Results.ToList();
                    db.Results.RemoveRange(listResults);
                    await db.SaveChangesAsync();
                }
            }
            return RedirectToAction("AddScan");
        }

        //Удаление данных у учащихся
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Students students = db.Students.Find(id);
            if (students == null)
            {
                return HttpNotFound();
            }
            List<Results> listResults = students.Results.ToList();
            db.Results.RemoveRange(listResults);
            db.SaveChanges();
            students.cipher = null;
            students.scan = null;
            students.last_change = DateTime.Now;
            db.Students.AddOrUpdate(students);
            db.SaveChanges();
            return RedirectToAction("AddCipher");
        }


        //Удаление бланка результата
        public ActionResult DeleteBlank(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Students students = db.Students.Find(id);
            if (students == null)
            {
                return HttpNotFound();
            }
            List<Results> listResults = students.Results.ToList();
            db.Results.RemoveRange(listResults);
            db.SaveChanges();
            students.scan = null;
            students.last_change = DateTime.Now;
            db.Students.AddOrUpdate(students);
            db.SaveChanges();
            return RedirectToAction("AddScan");
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
