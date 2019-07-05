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
    public class NioController : Controller
    {
        private rkrDBEntities db = new rkrDBEntities();

        // GET: Nio
        public ActionResult Index()
        {
            var students = db.Students.Where(s=>s.scan!=null).Include(s => s.Classes).Include(s => s.Genders);
            return View(students.ToList());
        }

        // GET: Nio/Details/5
        public ActionResult Details(int? id)
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
            return View(students);
        }

        // GET: Nio/Create
        public ActionResult Create()
        {
            ViewBag.id_classs = new SelectList(db.Classes, "id", "letter");
            ViewBag.id_gender = new SelectList(db.Genders, "id", "name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,cipher,surname,name,patronomic,mark_year,mark_1semestr,mark_2semestr,number_in_the_list,variant,scan,id_classs,id_gender,last_change")] Students students)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(students);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_classs = new SelectList(db.Classes, "id", "letter", students.id_classs);
            ViewBag.id_gender = new SelectList(db.Genders, "id", "name", students.id_gender);
            return View(students);
        }

        // GET: Nio/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.id_classs = new SelectList(db.Classes, "id", "letter", students.id_classs);
            ViewBag.id_gender = new SelectList(db.Genders, "id", "name", students.id_gender);
            return View(students);
        }

        // POST: Nio/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,cipher,surname,name,patronomic,mark_year,mark_1semestr,mark_2semestr,number_in_the_list,variant,scan,id_classs,id_gender,last_change")] Students students)
        {
            if (ModelState.IsValid)
            {
                db.Entry(students).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_classs = new SelectList(db.Classes, "id", "letter", students.id_classs);
            ViewBag.id_gender = new SelectList(db.Genders, "id", "name", students.id_gender);
            return View(students);
        }

        // GET: Nio/Delete/5
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
            return View(students);
        }


        public ActionResult IndexDate()
        {
            List <Time_X> times = db.Time_X.ToList();
            return View(times);
        }


        public ActionResult Files()
        {
            List<Documents> docs = db.Documents.ToList();
            return View(docs);
        }

        //Добавление документа (GET)
        public ActionResult AddFile()
        {
            Documents doc = new Documents();
            ViewBag.id_subject_number = new SelectList(db.Subjects_Numbers, "id", "subject");
            ViewBag.id_type_document = new SelectList(db.Types_Document, "id", "name");
            return View(doc);
        }

        //Добавление документа (POST)
        [HttpPost]
        public ActionResult AddFile(Documents documents, HttpPostedFileBase file)
        {
            documents.id = (db.Documents.Count() > 0) ? (db.Documents.Max(p => p.id) + 1) : 1;
            documents.date = DateTime.Now;
            if (file != null)
            {
                string fileName = System.IO.Path.GetFileName(file.FileName);
                file.SaveAs(Server.MapPath("~/Content/documents/" + fileName));
                documents.path = "/Content/documents/" + fileName;
                db.Documents.Add(documents);
                db.SaveChanges();
                return Redirect("/nio/Files");
            }
            ViewBag.id_subject_number = new SelectList(db.Subjects_Numbers, "id", "subject");
            ViewBag.id_type_document = new SelectList(db.Types_Document, "id", "name");
            return View(documents);
        }

        //Изменение документа (GET)
        public ActionResult EditFile(int?id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Documents doc = db.Documents.Find(id);
            if (doc == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_subject_number = new SelectList(db.Subjects_Numbers, "id", "subject", doc.id_subject_number);
            ViewBag.id_type_document = new SelectList(db.Types_Document, "id", "name", doc.id_type_document);
            return View(doc);
        }

        //Редактирование даты доступа (GET)
        public ActionResult EditDateX(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Time_X t = db.Time_X.Find(id);
            if (t == null)
            {
                return HttpNotFound();
            }
            return View(t);
        }

        //Редактирование даты доступа (POST)
        [HttpPost]
        public ActionResult EditDateX(Time_X t)
        {
            t.start = Convert.ToDateTime(Request.Form["start"]);
            t.finish = Convert.ToDateTime(Request.Form["finish"]);
            t.Subjects_Numbers = db.Subjects_Numbers.Find(t.id_subject_number);
            t.Roles = db.Roles.Find(t.id_role);
            db.Time_X.AddOrUpdate(t);
            db.SaveChanges();
            return Redirect("/Nio/IndexDate");
        }

        //Изменение документа (POST)
        [HttpPost]
        public ActionResult EditFile(Documents documents, HttpPostedFileBase file)
        {
            documents.date = DateTime.Now;
            if (file != null)
            {
                string fileName = System.IO.Path.GetFileName(file.FileName);
                file.SaveAs(Server.MapPath("~/Content/documents/" + fileName));
                documents.path = "/Content/documents/" + fileName;
            }
            db.Documents.AddOrUpdate(documents);
            db.SaveChanges();
            return Redirect("/nio/Files");
        }

        //Изменение документа (GET)
        public ActionResult DeleteFile(int id)
        {
            Documents doc = db.Documents.Find(id);
            db.Documents.Remove(doc);
            db.SaveChanges();
            return Redirect("/nio/Files");
        }


    }
}
