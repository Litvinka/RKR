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
using System.IO;
using Ionic.Zip;

namespace rkrApp.Controllers
{
    public class ResultsController : Controller
    {
        private rkrDBEntities db = new rkrDBEntities();

        //Список результатов
        public ActionResult _IndexResults(int? type, DateTime? finish)
        {
            int user_id = Convert.ToInt32(Session["user"]);
            int user_role = Convert.ToInt32(Session["role"]);
            ViewBag.finish = finish;
            List<Results> results = db.Results.Where(s => s.id_user == user_id && s.answer1 != null).ToList();
            if (type == 2)
            {
                results = results.Where(p => p.number_verification == 2).ToList();
            }
            else if (user_role == 5)
            {
                results = results.Where(p => p.number_verification == 1).ToList();
            }
            return PartialView(results);
        }

        //Просмотр результата по идентификатору
        public ActionResult _DetailsResult(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Results results = db.Results.Find(id);
            if (results == null)
            {
                return HttpNotFound();
            }
            return PartialView(results);
        }


        //Получение идентификатора результата
        public int GetIdResult()
        {
            string sadfsf = Session["user"].ToString();
            int _id_user = Convert.ToInt32(Session["user"]);
            int _id_student = Convert.ToInt32(Request.Form["Students_cipher"]);
            return db.Results.Where(a => a.id_student == _id_student && a.id_user == _id_user).First().id;
        }

        //Получение архива бланков результатов для проверки
        public void GetArchive(int?type)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                //zip.AddDirectoryByName("Files");
                int user_id = Convert.ToInt32(Session["user"]);
                List<Results> res = db.Results.Where(p => p.id_user == user_id && p.Students.scan!=null).ToList();
                if (type != null)
                {
                    res = res.Where(p=>p.number_verification==type).ToList();
                }
                foreach (Results r in res)
                {
                    string filePath = Server.MapPath("~" + r.Students.scan);
                    zip.AddFile(filePath,"");
                }
                Response.Clear();
                Response.BufferOutput = false;
                string zipName = String.Format("BlanksRKR.zip");
                Response.ContentType = "application/zip";
                Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                zip.Save(Response.OutputStream);
                Response.End();
            }
        }

        //Страница, на которой расположены ссылки на скачивание бланков результатов для проверки
        public ActionResult Scans()
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"]) != 5 && Convert.ToInt32(Session["role"]) != 6))
            {
                return Redirect("/");
            }
            int user_id = Convert.ToInt32(Session["user"]);
            int user_role = Convert.ToInt32(Session["role"]);
            if (user_role == 5)
            {
                int subject = db.Districts_Check.FirstOrDefault(p => p.id_user_check == user_id).id_subject_number;
                ViewBag.app_start = db.Time_X.FirstOrDefault(p => p.id_subject_number == subject && p.id_role == user_role && p.is_appeal == true).start;
                DateTime str = db.Time_X.FirstOrDefault(p => p.id_subject_number == subject && p.id_role == user_role && p.is_appeal == true).start;
            }
            return View();
        }

        //public void CheckResultsDistrict(int user_id)
        //{
        //    //определяем кол-во студентов, у которых есть скан, которые с этого района, но у которых нет результатов
        //    int count = db.Students
        //        .Where(s => s.scan != null &&
        //        s.Classes.Schools.Districts.Districts_Check.FirstOrDefault(f => f.id_user_check == user_id) != null &&
        //        s.Results.FirstOrDefault(r => r.number_verification == 1) == null).Count();
        //    //if (db.Results.Where(i => i.id_user == user_id).Count() == 0)
        //    //если кол-во больше нуля создаём результаты
        //    if (count > 0)
        //    {
        //        int first_id_result;
        //        if (db.Results.Count() != 0) { first_id_result = db.Results.Max(p => p.id) + 1; }
        //        else { first_id_result = 0; }
        //        List<Results> listResults = db.Districts_Check.Where(i => i.id_user_check == user_id).First().CreateResults(first_id_result);
        //        db.Results.AddRange(listResults);
        //        db.SaveChanges();
        //    }
        //}


        //public void CheckResultsArea(int user_id)
        //{
            //int id_subject = db.Areas_Check.First(i => i.id_user_check == user_id).id_subject_number;
            ////int count = db.Students
            ////   .Where(s => s.scan != null &&
            ////   s.Classes.Schools.Districts.Districts_Check.FirstOrDefault(f => f.id_user_check == user_id) != null &&
            ////   s.Results.FirstOrDefault(r => r.number_verification == 2) == null).Count();
            //if (db.Results.Where(i => i.number_verification == 2).Count() == 0)
            //{
            //    List<Areas_Check> listCheckers = db.Areas_Check.Where(i => i.id_subject_number == id_subject).ToList();
            //    //checker - проверяющая область
            //    List<Students> list = db.Students.Where(i => i.Classes.id_subject_number == id_subject).ToList();
            //    foreach (Areas_Check checker in listCheckers)
            //    {
            //        List<Students> listStudents = new List<Students>();
            //        // listStudents - список студентов проверяющей области
            //        foreach (Students student in list)
            //            if (student.IsForAreaChecker(checker) == true)
            //                listStudents.Add(student);
            //        int j = 0;
            //        // добавляем первого студента к первой области, второго ко второй и т.д. до седьмой, исключая область, к которой принадлежит студент
            //        for (int i = 0; i < listStudents.Count(); i++)
            //        {
            //            if (listCheckers[j].id_area != checker.id_area)
            //            {
            //                Results result = new Results();
            //                if (db.Results.Count() != 0) { result.id = db.Results.Max(p => p.id) + 1; }
            //                else { result.id = 0; }
            //                result.id_student = listStudents[i].id;
            //                result.id_user = listCheckers[j].id_user_check;
            //                result.number_verification = 2;
            //                db.Results.Add(result);
            //                db.SaveChanges();
            //            }
            //            else i--;
            //            if (j == listCheckers.Count() - 1) j = 0; else j++;
            //        }
            //        listStudents.Clear();
            //    }
            //    list.Clear();
            //    listCheckers.Clear();
            //}
        //}


        //public void CheckResultsAreaItog(int user_id)
        //{
        //    int id_subject = db.Areas_Check.First(i => i.id_user_check_itog == user_id).id_subject_number;
        //    if (db.Results.Where(i => i.number_verification == 3 && i.Students.Classes.id_subject_number==id_subject).Count() == 0)
        //    {
        //        List<Areas_Check> listCheckers = db.Areas_Check.Where(i=>i.id_subject_number==id_subject).ToList();
        //        //проверка нужно ли для этого студента создавать Result
        //        List<Students> list= db.Students.Where(i=>i.Classes.id_subject_number==id_subject).ToList();
        //        List<Students> listStudents = new List<Students>();
        //        foreach (Students student in list)
        //            if (student.IsForAreaItog == true)
        //                listStudents.Add(student);
        //        int j = 0;
        //        for (int i = 0; i < listStudents.Count(); i++)
        //        {
        //            //проверка области районной и областной проверки
        //            if (listStudents[i].IsForAreaItogChecher(listCheckers[j])==true)
        //            {
        //                Results result = new Results();
        //                if (db.Results.Count() != 0) { result.id = db.Results.Max(p => p.id) + 1; }
        //                else { result.id = 0; }
        //                result.id_student = listStudents[i].id;
        //                result.id_user = Convert.ToInt32(listCheckers[j].id_user_check_itog);
        //                result.number_verification = 3;
        //                db.Results.Add(result);
        //                db.SaveChanges();
        //            }
        //            else i--;
        //            if (j == listCheckers.Count()-1) j = 0; else j++;
        //        }
        //    }
        //}

        //Список документов для РКР по заданному предмету
        public ActionResult Works()
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"])!= 5 && Convert.ToInt32(Session["role"])!=6))
            {
                return Redirect("/");
            }
            int user_id = Convert.ToInt32(Session["user"]);
            int user_role = Convert.ToInt32(Session["role"]);
            int subject = 0;
            if (user_role == 5)
            {
                subject = db.Districts_Check.FirstOrDefault(p => p.id_user_check == user_id).Subjects_Numbers.id;
            }
            else
            {
                subject = db.Areas_Check.FirstOrDefault(p => p.id_user_check == user_id).Subjects_Numbers.id;
            }
            List<Documents> d = db.Documents.Where(p=>p.id_subject_number==subject).ToList();
            return View(d);
        }


        //Редактирование результата (Страница)
        public ActionResult EditResult()
        {
            int user_id = Convert.ToInt32(Session["user"]);
            int user_role = Convert.ToInt32(Session["role"]);
            List<Students> listStudents = new List<Students>();
            int subject = 0;
            if (user_role == 5)
            {
                subject = db.Districts_Check.FirstOrDefault(p => p.id_user_check == user_id).id_subject_number;
                listStudents =db.Students.Where(a => a.Results.Where(b => b.id_user == user_id && b.number_verification == 1).Count() != 0).OrderBy(p => p.cipher).ToList();
                ViewBag.Add = listStudents.Where(a => a.Results.Where(b => b.id_user == user_id && b.answer1 != null && b.number_verification == 1).Count() != 0).Count();
                ViewBag.All = listStudents.Where(a => a.Results.Where(b => b.id_user == user_id && b.number_verification == 1).Count() != 0).Count();
                listStudents = listStudents.Where(p => p.Results.Where(b => b.id_user == user_id && b.number_verification == 1 && b.answer1 == null).Count() != 0).ToList();
            }
            else
            {
                subject = db.Areas_Check.FirstOrDefault(p => p.id_user_check == user_id).id_subject_number;
                listStudents = db.Students.Where(a => a.Results.Where(b => b.id_user == user_id && b.number_verification == 3).Count() != 0).OrderBy(p => p.cipher).ToList();
                ViewBag.Add = listStudents.Where(a => a.Results.Where(b => b.id_user == user_id && b.answer1 != null && b.number_verification == 3).Count() != 0).Count();
                ViewBag.All = listStudents.Where(a => a.Results.Where(b => b.id_user == user_id && b.number_verification == 3).Count() != 0).Count();
                listStudents = listStudents.Where(p => p.Results.Where(b => b.id_user == user_id && b.number_verification == 3 && b.answer1 == null).Count() != 0).ToList();
            }
            Time_X t = db.Time_X.FirstOrDefault(p => p.id_subject_number == subject && p.id_role == user_role && p.is_appeal == null);
            ViewBag.start = t.start;
            ViewBag.finish = t.finish;
            ViewBag.Students_cipher = new SelectList(listStudents, "id", "cipher");
            return View();
        }


        //Редактирование результата проверки работы, отправленной на апелляцию (Страница)
        public ActionResult EditResultAppeal()
        {
            int user_id = Convert.ToInt32(Session["user"]);
            int user_role = Convert.ToInt32(Session["role"]);

            int subject = db.Districts_Check.FirstOrDefault(p => p.id_user_check == user_id).id_subject_number;
            Time_X t = db.Time_X.FirstOrDefault(p => p.id_subject_number == subject && p.id_role == user_role && p.is_appeal == true);
            ViewBag.start = t.start;
            ViewBag.finish = t.finish;

            List<Students> listStudents = db.Students.Where(a => a.Results.Where(b => b.id_user == user_id && b.number_verification == 2).Count() != 0).OrderBy(p => p.cipher).ToList();
            ViewBag.Add = listStudents.Where(a => a.Results.Where(b => b.answer1 != null && b.number_verification==2).Count() != 0).Count();
            ViewBag.All = listStudents.Where(a => a.Results.Where(b => b.number_verification == 2).Count() != 0).Count();
            
            ViewBag.Students_cipher = new SelectList(listStudents.Where(p => p.Results.Where(b => b.id_user == user_id && b.number_verification == 2 && b.answer1 == null).Count() != 0), "id", "cipher");
            return View();
        }


        //Редактирование результата (Изменение данных в бд)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditResult([Bind(Include = "answer1,answer2,answer3,answer4,answer5,answer6,answer7,answer8,answer9,answer10,errors1,errors2,errors3,errors4,errors5,errors6,errors7,errors8,errors9,errors10")] Results results)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int _id_user = Convert.ToInt32(Session["user"]);
                    int _id_role = Convert.ToInt32(Session["role"]);
                    int _id_student = Convert.ToInt32(Request.Form["Students_cipher"]);
                    int _id = GetIdResult();
                    Byte _number_verification = db.Results.Where(s => s.id == _id).First().number_verification;
                    results.id = _id;
                    results.id_student = _id_student;
                    results.id_user = _id_user;
                    results.number_verification = _number_verification;
                    results.date_verification = DateTime.Now;
                    db.Results.AddOrUpdate(results);
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }
                return RedirectToAction("EditResult");
            }
            ViewBag.id_student = new SelectList(db.Students, "id", "cipher", results.id_student);
            return View(results);
        }

        //Редактирование результата проверки работы, отправленной на апелляцию (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditResultAppeal([Bind(Include = "answer1,answer2,answer3,answer4,answer5,answer6,answer7,answer8,answer9,answer10,errors1,errors2,errors3,errors4,errors5,errors6,errors7,errors8,errors9,errors10")] Results results)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int _id_user = Convert.ToInt32(Session["user"]);
                    int _id_role = Convert.ToInt32(Session["role"]);
                    int _id_student = Convert.ToInt32(Request.Form["Students_cipher"]);
                    Results r = db.Results.First(p => p.id_user == _id_user && p.id_student == _id_student && p.number_verification == 2);
                    results.id = r.id;
                    results.id_student = _id_student;
                    results.id_user = _id_user;
                    results.number_verification = r.number_verification;
                    results.date_verification = DateTime.Now;
                    db.Results.AddOrUpdate(results);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }
                return RedirectToAction("EditResultAppeal");
            }
            ViewBag.id_student = new SelectList(db.Students, "id", "cipher", results.id_student);
            return View(results);
        }


        // GET: Results1/Edit/5
        public ActionResult EditResultAfter(int? id)
        {
            int user_id = Convert.ToInt32(Session["user"]);
            int user_role = Convert.ToInt32(Session["role"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Results results = db.Results.Find(id);
            if (results == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_student = new SelectList(db.Students, "id", "cipher", results.id_student);
            ViewBag.id_user = new SelectList(db.Users, "id", "password", results.id_user);
            return View(results);
        }

        // POST: Results1/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditResultAfter([Bind(Include = "id,id_student,answer1,answer2,answer3,answer4,answer5,answer6,answer7,answer8,answer9,answer10,errors1,errors2,errors3,errors4,errors5,errors6,errors7,errors8,errors9,errors10,id_user,number_verification,date_verification")] Results results)
        {
            int _id_role = Convert.ToInt32(Session["role"]);
            if (ModelState.IsValid)
            {
                db.Results.AddOrUpdate(results);
                db.SaveChanges();
                return RedirectToAction("EditResult");
            }
            ViewBag.id_student = new SelectList(db.Students, "id", "cipher", results.id_student);
            ViewBag.id_user = new SelectList(db.Users, "id", "password", results.id_user);
            return View(results);
        }
        
        //Обнуление результата
        public ActionResult DeleteResult(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Results results = db.Results.Find(id); //поиск результата
            if (results == null)
            {
                return HttpNotFound();
            }
            results.answer1 = null;
            results.answer2 = null;
            results.answer3 = null;
            results.answer4 = null;
            results.answer5 = null;
            results.answer6 = null;
            results.answer7 = null;
            results.answer8 = null;
            results.answer9 = null;
            results.answer10 = null;
            results.errors1 = null;
            results.errors2 = null;
            results.errors3 = null;
            results.errors4 = null;
            results.errors5 = null;
            results.errors6 = null;
            results.errors7 = null;
            results.errors8 = null;
            results.errors9 = null;
            results.errors10 = null;
            results.date_verification = null;
            int _id_role = Convert.ToInt32(Session["role"]);
            db.SaveChanges();
            return RedirectToAction("EditResult");
        }

        //Получение ссылки на бланк результатов РКР заданного учащегося в базе данных
        [HttpPost]
        public JsonResult GetScan(string param)
        {
            int p = Convert.ToInt32(param);
            Students std = db.Students.Find(p);
            String url = (std.scan != null) ? std.scan : "";
            var d = System.Web.Helpers.Json.Encode(url);
            return Json(d);
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
