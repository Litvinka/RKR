using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using rkrApp.Models;

namespace rkrApp.Controllers
{
    public class SchoolsController : Controller
    {
        private rkrDBEntities db = new rkrDBEntities();

        // GET: Schools
        public ActionResult Index()
        {
            var schools = db.Schools.Include(s => s.Districts).Include(s => s.Places).Include(s => s.Types_edu).Include(s => s.Users);
            return View(schools.ToList());
        }

        // GET: Schools/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schools schools = db.Schools.Find(id);
            if (schools == null)
            {
                return HttpNotFound();
            }
            return View(schools);
        }

        // GET: Schools/Create
        public ActionResult Create()
        {
            ViewBag.id_district = new SelectList(db.Districts, "id", "name");
            ViewBag.id_place = new SelectList(db.Places, "id", "name");
            ViewBag.id_type_edy = new SelectList(db.Types_edu, "id", "name");
            ViewBag.id_user = new SelectList(db.Users, "id", "password");
            return View();
        }

        // POST: Schools/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,id_district,id_user,id_type_edy,id_place")] Schools schools)
        {
            if (ModelState.IsValid)
            {
                db.Schools.Add(schools);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_district = new SelectList(db.Districts, "id", "name", schools.id_district);
            ViewBag.id_place = new SelectList(db.Places, "id", "name", schools.id_place);
            ViewBag.id_type_edy = new SelectList(db.Types_edu, "id", "name", schools.id_type_edy);
            ViewBag.id_user = new SelectList(db.Users, "id", "password", schools.id_user);
            return View(schools);
        }

        // GET: Schools/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schools schools = db.Schools.Find(id);
            if (schools == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_district = new SelectList(db.Districts, "id", "name", schools.id_district);
            ViewBag.id_place = new SelectList(db.Places, "id", "name", schools.id_place);
            ViewBag.id_type_edy = new SelectList(db.Types_edu, "id", "name", schools.id_type_edy);
            ViewBag.id_user = new SelectList(db.Users, "id", "password", schools.id_user);
            return View(schools);
        }

        // POST: Schools/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,id_district,id_user,id_type_edy,id_place")] Schools schools)
        {
            if (ModelState.IsValid)
            {
                db.Entry(schools).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_district = new SelectList(db.Districts, "id", "name", schools.id_district);
            ViewBag.id_place = new SelectList(db.Places, "id", "name", schools.id_place);
            ViewBag.id_type_edy = new SelectList(db.Types_edu, "id", "name", schools.id_type_edy);
            ViewBag.id_user = new SelectList(db.Users, "id", "password", schools.id_user);
            return View(schools);
        }

        // GET: Schools/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schools schools = db.Schools.Find(id);
            if (schools == null)
            {
                return HttpNotFound();
            }
            return View(schools);
        }

        // POST: Schools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Schools schools = db.Schools.Find(id);
            db.Schools.Remove(schools);
            db.SaveChanges();
            return RedirectToAction("Index");
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
