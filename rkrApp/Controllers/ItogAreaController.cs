using rkrApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace rkrApp.Controllers
{
    public class ItogAreaController : Controller
    {
        private rkrDBEntities db = new rkrDBEntities();
        // GET: ItogArea
        public ActionResult Index()
        {
            int user = Convert.ToInt32(Session["user"]);
            var res = db.Results.Where(p=>p.number_verification==3 && p.id_user==user && p.answer1 == null && p.answer2 == null && p.answer3 == null && p.answer4 == null && p.answer5 == null && p.answer6 == null && p.answer7 == null && p.answer8 == null && p.answer9 == null && p.answer10 == null);
            List<Results> r1 = new List<Results>();
            List<Results> r2 = new List<Results>();
            foreach(Results item in res)
            {
                r1.Add(db.Results.First(p => p.id_student == item.id_student && p.number_verification==1));
                r2.Add(db.Results.First(p => p.id_student == item.id_student && p.number_verification == 2));
            }
            ViewBag.r2 = r2;
            return View(r1);
        }


        public ActionResult _Corrected()
        {
            int user = Convert.ToInt32(Session["user"]);
            var res = db.Results.Where(p => p.number_verification == 3 && p.id_user == user && (p.answer1!=null || p.answer2 != null || p.answer3 != null || p.answer4 != null || p.answer5 != null || p.answer6 != null || p.answer7 != null || p.answer8 != null || p.answer9 != null || p.answer10 != null));
            List<Results> r2 = new List<Results>();
            foreach (Results item in res)
            {
                r2.Add(db.Results.First(p => p.id_student == item.id_student && p.number_verification == 2));
            }
            ViewBag.r2 = r2;
            return PartialView(res);
        }


        // GET: ItogArea/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ItogArea/Create
        public ActionResult Create(int student)
        {
            var res=db.Results.First(p => p.id_student == student && p.number_verification == 3);
            ViewBag.r1 = db.Results.First(p => p.id_student == student && p.number_verification == 1);
            ViewBag.r2 = db.Results.First(p => p.id_student == student && p.number_verification == 2);
            return View(res);
        }

        // POST: ItogArea/Create
        [HttpPost]
        public ActionResult Create(Results result)
        {
            db.Results.AddOrUpdate(result);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: ItogArea/Edit/5
        public ActionResult Edit(int student)
        {

            var res = db.Results.First(p => p.id_student == student && p.number_verification == 3);
            ViewBag.r1 = db.Results.First(p => p.id_student == student && p.number_verification == 1);
            ViewBag.r2 = db.Results.First(p => p.id_student == student && p.number_verification == 2);
            return View(res);
        }

        // POST: ItogArea/Edit/5
        [HttpPost]
        public ActionResult Edit(Results result)
        {
            db.Results.AddOrUpdate(result);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: ItogArea/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ItogArea/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
