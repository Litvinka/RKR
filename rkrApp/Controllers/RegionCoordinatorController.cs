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
    public class RegionCoordinatorController : Controller
    {
        private rkrDBEntities db = new rkrDBEntities();

        // GET: RegionCoordinator
        public ActionResult Index()
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"])!=7 && Convert.ToInt32(Session["role"])!=1))
            {
                return Redirect("/");
            }
            int user_id = Convert.ToInt32(Session["user"]);
            int role = Convert.ToInt32(Session["role"]);
            List<Districts> districts = new List<Districts>();
            if (role == 7)
            {
                int area_id = db.Area.FirstOrDefault(p => p.id_coordinator == user_id).id;
                districts = db.Districts.Where(p => p.id_area == area_id).ToList();
            }
            else
            {
                districts = db.Districts.ToList();
            }
            return View(districts);
        }

        public ActionResult AllSchool()
        {
            int user_id = Convert.ToInt32(Session["user"]);
            int role = Convert.ToInt32(Session["role"]);
            List<Schools> schools = new List<Schools>();
            if (role == 7)
            {
                int area_id = db.Area.FirstOrDefault(p => p.id_coordinator == user_id).id;
                schools = db.Schools.Where(p => p.Districts.id_area == area_id).ToList();
            }
            else
            {
                schools = db.Schools.ToList();
            }
            return PartialView(schools);
        }


        public ActionResult Class(int id)
        {
            List<Classes> c = db.Classes.Where(p => p.id_school == id).ToList();
            return View(c);
        }


        public ActionResult Students(int id)
        {
            List<Students> s = db.Students.Where(p => p.id_classs == id).OrderBy(p=>p.number_in_the_list).ToList();
            return PartialView(s);
        }


        public ActionResult AllArea()
        {
            List<Area> areas = db.Area.ToList();
            return PartialView(areas);
        }

    }
}
