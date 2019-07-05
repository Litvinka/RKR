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
    public class CoordinatorController : Controller
    {
        private rkrDBEntities db = new rkrDBEntities();

        //Получение идентификатора школы
        public int GetIdSclool()
        {
            int user_id = Convert.ToInt32(Session["user"]);
            return db.Schools.Where(s => s.id_user == user_id).First().id;
        }
        
        //Получение подробной информации о школе
        public ActionResult _DetailsSchool()
        {
            int id = GetIdSclool();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schools schools = db.Schools.Find(id);
            if (schools == null)
            {
                return HttpNotFound();
            }
            return PartialView(schools);
        }

        //Редактирование данных о школе (GET)
        public ActionResult EditSchool()
        {
            int id = GetIdSclool();
            Schools schools = db.Schools.Find(id);
            if (schools == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_place = new SelectList(db.Places, "id", "name", schools.id_place);
            ViewBag.id_type_edy = new SelectList(db.Types_edu, "id", "name", schools.id_type_edy);
            return View(schools);
        }

        //Редактирование данных о школе (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSchool([Bind(Include = "name,id_district,id_type_edy,id_place,code")] Schools schools)
        {
            if (ModelState.IsValid)
            {
                schools.id = GetIdSclool();
                schools.id_user = Convert.ToInt32(Session["user"]);
                db.Schools.AddOrUpdate(schools);
                db.SaveChanges();
                return RedirectToAction("IndexClasses");
            }
            ViewBag.id_place = new SelectList(db.Places, "id", "name", schools.id_place);
            ViewBag.id_type_edy = new SelectList(db.Types_edu, "id", "name", schools.id_type_edy);
            return View(schools);
        }

        //Список классов данной школы
        public ActionResult IndexClasses()
        {
            int id_school = GetIdSclool();
            var classes = db.Classes.Include(c => c.Subjects_Numbers).Include(c => c.Schools).Where(s => s.Schools.id == id_school);
            return View(classes.ToList());
        }

        //Страница, на который школьный координатор может подать работу учащегося на апелляцию
        public ActionResult Appeal(int school)
        {
            Classes c = db.Schools.First(p => p.id == school).Classes.FirstOrDefault();
            if (c != null)
            {
                int subject = c.id_subject_number;
                ViewBag.start = db.Time_X.FirstOrDefault(p => p.id_subject_number == subject && p.id_role == 5 && p.is_appeal==null).finish;
                ViewBag.end = db.Time_X.FirstOrDefault(p => p.id_subject_number == subject && p.id_role == 5 && p.is_appeal==true).start;
            }
            List<Students> std = db.Students.Where(p => p.Classes.id_school == school).ToList();
            return View(std);
        }

        //Результаты районной проверки, апелляции и итоговая оценка за работу
        public ActionResult Results(int school)
        {
            Classes c = db.Schools.First(p => p.id == school).Classes.FirstOrDefault();
            if (c != null)
            {
                int subject = c.id_subject_number;
                ViewBag.time = db.Time_X.FirstOrDefault(p => p.id_subject_number == subject && p.id_role == 5 && p.is_appeal == true).finish;
            }
            List<Students> std = db.Students.Where(p => p.Classes.id_school == school).ToList();
            return View(std);
        }


        //Подробное описание класса по идентификатору
        public ActionResult _DetailsClass(int? id)
        {
            //id = 1;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Classes _class = db.Classes.Find(id);
            if (_class == null)
            {
                return HttpNotFound();
            }
            return PartialView(_class);
        }

        //Добавление или редактирование класса (GET)
        public ActionResult _CreateOrEditClass(int? id)
        {
            if (id == null)
            {
                ViewBag.id_subject_number = new SelectList(db.Subjects_Numbers, "id", "subject_number");
                ViewBag.Caption = "Добавление информации о классе:";
                return PartialView();
            }
            Classes _class = db.Classes.Find(id);
            if (_class == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_subject_number = new SelectList(db.Subjects_Numbers, "id", "subject_number", _class.id_subject_number);
            ViewBag.Caption = "Редактирование информации о классе:";
            return PartialView(_class);
        }

        //Создание нового класса (GET)
        public ActionResult CreateClass()
        {
            return View();
        }

        //Создание нового класса (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateClass([Bind(Include = "letter,id_subject_number,count")] Classes classes)
        {
            if (ModelState.IsValid)
            {
                if (db.Classes.Count() != 0) { classes.id = db.Classes.Max(p => p.id) + 1; }
                else { classes.id = 1; }
                classes.id_school = GetIdSclool();
                db.Classes.Add(classes);
                db.SaveChanges();
                return RedirectToAction("IndexClasses");
            }
            return View();
        }

        //Редактирование информации о классе (GET)
        public ActionResult EditClass()
        {
            return View();
        }

        //Редактирование информации о классе (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClass([Bind(Include = "id,letter,id_subject_number,count")] Classes classes)
        {
            if (ModelState.IsValid)
            {
                classes.id_school = GetIdSclool();
                db.Entry(classes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexClasses");
            }
            return View();
        }

        //Удаление класса и всех учащихся из него из бд
        public ActionResult DeleteClass(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Classes classes = db.Classes.Find(id);
            if (classes == null)
            {
                return HttpNotFound();
            }
            List<Students> listStudents = db.Students.Where(s => s.id_classs == id).ToList();
            foreach (Students student in listStudents)
            {
                List<Results> listResults = db.Results.Where(s => s.id_student == student.id).ToList();
                db.Results.RemoveRange(listResults);
            }
            db.Students.RemoveRange(listStudents);
            db.Classes.Remove(classes);
            db.SaveChanges();
            return RedirectToAction("IndexClasses");
        }

        //Подтверждение об удалении класса
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedClass(int id)
        {
            Classes classes = db.Classes.Find(id);
            db.Classes.Remove(classes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Список учащихся данного класса
        public ActionResult _IndexStudents(int? id)
        {
            var students = db.Students.Include(s => s.Classes).Include(s => s.Genders).Where(s => s.id_classs == id);
            return PartialView(students.ToList());
        }

        //Отправка на апелляцию работы. Создается новый пустой результат с номером проверки = 2
        public ActionResult AddAppeal(int id)
        {
            int school_id = db.Students.Find(id).Classes.id_school;
            Results r = new Results();
            Results first = db.Results.FirstOrDefault(p => p.id_student == id && (p.number_verification==1 || p.number_verification==0));
            if (first != null)
            {
                r.id = db.Results.Max(p => p.id) + 1;
                r.id_student = id;
                r.number_verification = 2;
                r.id_user = first.id_user;
                db.Results.Add(r);
                db.SaveChanges();
            }
            return Redirect("/Coordinator/Appeal?school="+school_id);
        }

        //Отменить отправку работы на апелляцию. Удаляется созданный ранее результат с номером проверки = 2
        public ActionResult DelAppeal(int id)
        {
            int school_id = db.Students.Find(id).Classes.id_school;
            Results r = db.Results.First(p => p.id_student == id && p.number_verification == 2);
            db.Results.Remove(r);
            db.SaveChanges();
            return Redirect("/Coordinator/Appeal?school=" + school_id);
        }

        //Добавление или редактирование информации об учащемся
        public ActionResult _CreateOrEditStudent(int? id)
        {
            //id = 1;
            if (id == null)
            {
                ViewBag.id_classs = new SelectList(db.Classes, "id", "letter");
                ViewBag.id_gender = new SelectList(db.Genders, "id", "name");
                return PartialView();
            }
            Students _student = db.Students.Find(id);
            if (_student == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_classs = new SelectList(db.Classes, "id", "letter", _student.id_classs);
            ViewBag.id_gender = new SelectList(db.Genders, "id", "name", _student.id_gender);
            return PartialView(_student);
        }
        
        //Добавление нового учащегося (GET)
        public ActionResult CreateStudent(int?id)
        {
            ViewBag.time = db.Classes.Where(s => s.id == id).First().Subjects_Numbers.start.Value;
            ViewBag.id_classs = new SelectList(db.Classes, "id", "letter");
            ViewBag.id_gender = new SelectList(db.Genders, "id", "name");
            return View();
        }

        //Добавление нового учащегося (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStudent([Bind(Include = "surname,name,patronomic,level_edu,mark_year,mark_1semestr,mark_2semestr,number_in_the_list,id_gender")] Students students)
        {
            if (ModelState.IsValid)
            {
                if (db.Students.Count() != 0) { students.id = db.Students.Max(p => p.id) + 1; }
                else { students.id = 1; }
                students.id_classs = Convert.ToInt32(Request.Form["id"]);
                db.Students.Add(students);
                db.SaveChanges();
                return RedirectToAction("CreateStudent");
            }
            return View();
        }

        //Редактирование информации об учащемся (GET)
        public ActionResult EditStudent(int? id)
        {
            Students _student = db.Students.Find(id);
            if (_student == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_classs = new SelectList(db.Classes, "id", "letter", _student.id_classs);
            ViewBag.id_gender = new SelectList(db.Genders, "id", "name", _student.id_gender);
            return View(_student);
        }

        //Редактирование информации об учащемся (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudent([Bind(Include = "id,surname,name,patronomic,level_edu,mark_year,mark_1semestr,mark_2semestr,number_in_the_list,id_gender,id_classs")] Students students)
        {
            if (ModelState.IsValid)
            {
                db.Students.AddOrUpdate(students);
                db.SaveChanges();
                return RedirectToAction("CreateStudent", new { id = students.id_classs });
            }
            return View();
        }

        //Удаление информации об учащемся из базы данных
        public ActionResult DeleteStudent(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Students student = db.Students.Find(id);
            int id_class = student.id_classs;
            Subjects_Numbers subj = db.Subjects_Numbers.Find(student.Classes.id_subject_number);
            if (student == null || DateTime.Now>subj.start)
            {
                return HttpNotFound();
            }
            List<Results> listResults = db.Results.Where(s => s.id_student == id).ToList();
            db.Results.RemoveRange(listResults);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("CreateStudent", new { id = id_class });
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
