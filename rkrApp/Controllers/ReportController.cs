using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rkrApp.Models;
using System.Data.Entity.Migrations;
using OfficeOpenXml;
using System.IO;

namespace rkrApp.Controllers
{
    public class ReportController : Controller
    {
        private rkrDBEntities db = new rkrDBEntities();
        // GET: Report
        public ActionResult Index()
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"]) < 5 && Convert.ToInt32(Session["role"]) > 1))
            {
                return Redirect("/");
            }
            else if (Convert.ToInt32(Session["role"]) == 1 || Convert.ToInt32(Session["role"]) == 7)
            {
                return Redirect("/Report/Report");
            }
            int user_id = Convert.ToInt32(Session["user"]);
            Users user = db.Users.Find(user_id);
            List<Students> st = new List<Students>();

            if (user.id_role == 5)
            {
                Districts_Check dc = db.Districts_Check.FirstOrDefault(p=>p.id_user_check==user.id);
                st = db.Students.Where(p=>p.Classes.Schools.id_district==dc.id_district && p.Classes.id_subject_number==dc.id_subject_number && p.cipher != null && p.scan!=null && p.Results.Count() >= 2).ToList();
                ViewBag.subject = dc.Subjects_Numbers.subject;
            }
            else if (user.id_role == 6)
            {
                Areas_Check ac = db.Areas_Check.FirstOrDefault(p => p.id_user_check == user.id);
                st = db.Students.Where(p => p.Classes.Schools.Districts.id_area == ac.id_area && p.Classes.id_subject_number == ac.id_subject_number && p.cipher != null && p.scan != null && p.Results.Count()>=2).ToList();
                ViewBag.subject = ac.Subjects_Numbers.subject;
            }
            return View(st);
        }

        //Список учащихся
        public ActionResult AllPupils()
        {
            if (Session["role"] == null || Convert.ToInt32(Session["role"]) != 1)
            {
                return Redirect("/");
            }
            List<Students> st = db.Students.ToList();
            return View(st);
        }


        public ActionResult Result(int subject)
        {
            if (Session["role"] == null || (Convert.ToInt32(Session["role"]) != 1 && Convert.ToInt32(Session["role"]) != 7))
            {
                return Redirect("/");
            }
            int user_id = Convert.ToInt32(Session["user"]);
            Users user = db.Users.Find(user_id);
            List<Students> st = new List<Students>();

            if (user.id_role == 1)
            {
                st = db.Students.Where(p => p.Classes.id_subject_number == subject && p.cipher != null && p.scan != null && p.Results.Count() >= 2).ToList();
            }
            else if (user.id_role == 7)
            {
                Area ac = db.Area.FirstOrDefault(p => p.id_coordinator == user.id);
                st = db.Students.Where(p => p.Classes.Schools.Districts.id_area == ac.id && p.Classes.id_subject_number==subject && p.cipher != null && p.scan != null && p.Results.Count() >= 2).ToList();
            }
            ViewBag.subject = db.Subjects_Numbers.Find(subject).subject;
            return View(st);
        }

        /* Итоговые результаты проверки РПК (если была апелляция, учитывается оценка за нее, если нет - оценка за первую проверку РПК) */
        public ActionResult ExportExcel(int subject)
        {
            int rows = 3, cols = 1;
            Subjects_Numbers s = db.Subjects_Numbers.Find(subject);
            int user_id = Convert.ToInt32(Session["user"]);
            Users user = db.Users.Find(user_id);
            List<Students> st = new List<Students>();
            if (user.id_role == 1)
            {
                st = db.Students.Where(p => p.Classes.id_subject_number == subject && p.cipher != null && p.scan != null && p.Results.Count() >= 1).ToList();
            }
            else if (user.id_role == 7)
            {
                Area ac = db.Area.FirstOrDefault(p => p.id_coordinator == user.id);
                st = db.Students.Where(p => p.Classes.Schools.Districts.id_area == ac.id && p.Classes.id_subject_number == subject && p.cipher != null && p.scan != null && p.Results.Count() >= 1).ToList();
            }
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Результаты_"+s.subject);
            workSheet.Cells[1, 1].Value = "№ п/п";
            workSheet.Cells[1, 2].Value = "Название региона";
            workSheet.Cells[1, 3].Value = "Название района";
            workSheet.Cells[1, 4].Value = "Название учреждения образования";
            workSheet.Cells[1, 5].Value = "Код учреждения образования";
            workSheet.Cells[1, 6].Value = "Место расположения учреждения образования (г/с)";
            workSheet.Cells[1, 7].Value = "Вид учреждения образования";
            workSheet.Cells[1, 8].Value = "ФИО учащегося";
            workSheet.Cells[1, 9].Value = "Пол учащегося";
            workSheet.Cells[1, 10].Value = "Уровень";
            workSheet.Cells[1, 11].Value = "Класс (с указанием буквы)";
            workSheet.Cells[1, 12].Value = "Шифр работы";
            workSheet.Cells[1, 13].Value = "Вариант РКР";
            workSheet.Cells[1, 14].Value = "Количество баллов, полученных учащимися за задания";
            workSheet.Cells[1, 24].Value = "Ошибки";
            workSheet.Cells[1, 34].Value = "Суммарный балл за РКР (автоматизированная система перепроверяет корректность ввода суммы баллов за РКР)";
            workSheet.Cells[1, 35].Value = "Отметка за РКР (выставляется на основании суммарного балла за РКР)";
            workSheet.Cells[1, 36].Value = "Уровень усвоения учебного материала по результатам РКР (выставляется на основании суммарного балла)";
            workSheet.Cells[1, 37].Value = "Результаты итоговой аттестации по учебному предмету за 2017/18 уч. г.(отметка)";
            workSheet.Cells[1, 38].Value = "Уровень усвоения учебного материала по результатам итоговой аттестации за 2017/18 уч. г.";
            workSheet.Cells[1, 39].Value = "Результаты промежуточной аттестации за ׀ четверть 2018/19 уч. г. (отметка)";
            workSheet.Cells[1, 40].Value = "Уровень усвоения учебного материала по результатам промежуточной аттестации за ׀ четверть 2018/19 уч. г.";
            workSheet.Cells[1, 41].Value = "Результаты промежуточной аттестации за ׀׀ четверть 2018/19 уч. г. (отметка)";
            workSheet.Cells[1, 42].Value = "Уровень усвоения учебного материала по результатам итоговой аттестации за ׀׀ четверть 2018/19 уч. г.";
            workSheet.Cells[1, 43].Value = "Проверяющий. Email";
            workSheet.Cells[1, 44].Value = "Проверяющий. Область";
            workSheet.Cells[1, 45].Value = "Проверяющий. Район";
            workSheet.Cells[2, 14].Value = "№ 1";
            workSheet.Cells[2, 15].Value = "№ 2";
            workSheet.Cells[2, 16].Value = "№ 3";
            workSheet.Cells[2, 17].Value = "№ 4";
            workSheet.Cells[2, 18].Value = "№ 5";
            workSheet.Cells[2, 19].Value = "№ 6";
            workSheet.Cells[2, 20].Value = "№ 7";
            workSheet.Cells[2, 21].Value = "№ 8";
            workSheet.Cells[2, 22].Value = "№ 9";
            workSheet.Cells[2, 23].Value = "№ 10";
            workSheet.Cells[2, 24].Value = "№ 1";
            workSheet.Cells[2, 25].Value = "№ 2";
            workSheet.Cells[2, 26].Value = "№ 3";
            workSheet.Cells[2, 27].Value = "№ 4";
            workSheet.Cells[2, 28].Value = "№ 5";
            workSheet.Cells[2, 29].Value = "№ 6";
            workSheet.Cells[2, 30].Value = "№ 7";
            workSheet.Cells[2, 31].Value = "№ 8";
            workSheet.Cells[2, 32].Value = "№ 9";
            workSheet.Cells[2, 33].Value = "№ 10";

            for(int i = 0; i < st.Count(); ++i)
            {
                workSheet.Cells[rows, cols++].Value = (i+1);
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.Districts.Area.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.Districts.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.code;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.Places.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.Types_edu.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).surname+" "+st.ElementAt(i).name + " " + st.ElementAt(i).patronomic;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Genders.name;
                workSheet.Cells[rows, cols++].Value = (Convert.ToBoolean(st.ElementAt(i).level_edu)) ? "Повышенный" : "Базовый";
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Classes.letter != null) ? (st.ElementAt(i).Classes.Subjects_Numbers.number+" "+ st.ElementAt(i).Classes.letter) : st.ElementAt(i).Classes.Subjects_Numbers.number;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).cipher;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).getVariant();
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).answer1 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer1;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).answer2 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer2;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).answer3 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer3;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).answer4 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer4;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).answer5 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer5;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).answer6 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer6;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).answer7 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer7;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).answer8 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer8;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).answer9 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer9;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).answer10 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer10;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).errors1 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors1;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).errors2 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors2;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).errors3 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors3;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).errors4 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors4;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).errors5 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors5;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).errors6 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors6;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).errors7 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors7;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).errors8 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors8;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).errors9 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors9;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).errors10 : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors10;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).SumOfScores : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).SumOfScores;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).Mark : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).Mark;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2) != null) ? st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 2).Level : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).Level;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).mark_year;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).getLevel(st.ElementAt(i).mark_year);
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).mark_1semestr;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).getLevel(st.ElementAt(i).mark_1semestr);
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).mark_2semestr;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).getLevel(st.ElementAt(i).mark_2semestr);
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p=>p.number_verification==1).Users.email;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).Users.Districts_Check.First().Districts.Area.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).Users.Districts_Check.First().Districts.name;
                rows++;
                cols = 1;
            }
            if (user.id_role == 1)
            {
                rows += 5;
                workSheet.Cells[rows, 12].Value = "количество учащихся";
                workSheet.Cells[rows, 13].Value = "набравших максимальный балл";
                workSheet.Cells[rows, 14].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer1 != null && t.answer1.Equals("1")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer1 != null && t.answer1.Equals("1")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 15].Value = (subject != 2) ? (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer2 != null && t.answer2.Equals("1")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer2 != null && t.answer2.Equals("1")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0))) : (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer2 != null && t.answer2.Equals("2")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer2 != null && t.answer2.Equals("2")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 16].Value = (subject != 2) ? (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer3 != null && t.answer3.Equals("2")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer3 != null && t.answer3.Equals("2")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0))) : 0;
                workSheet.Cells[rows, 17].Value = (subject != 2) ? (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer4 != null && t.answer4.Equals("2")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer4 != null && t.answer4.Equals("2")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0))) : (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer4 != null && t.answer4.Equals("3")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer4 != null && t.answer4.Equals("3")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 18].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer5 != null && t.answer5.Equals("3")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer5 != null && t.answer5.Equals("3")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 19].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer6 != null && t.answer6.Equals("3")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer6 != null && t.answer6.Equals("3")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 20].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer7 != null && t.answer7.Equals("4")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer7 != null && t.answer7.Equals("4")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 21].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer8 != null && t.answer8.Equals("4")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer8 != null && t.answer8.Equals("4")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 22].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer9 != null && t.answer9.Equals("5")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer9 != null && t.answer9.Equals("5")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows++, 23].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer10 != null && t.answer10.Equals("5")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer10 != null && t.answer10.Equals("5")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 13].Value = "не приступивших к выполнению";
                workSheet.Cells[rows, 14].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer1 != null && t.answer1.Equals("н")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer1 != null && t.answer1.Equals("н")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 15].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer2 != null && t.answer2.Equals("н")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer2 != null && t.answer2.Equals("н")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 16].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer3 != null && t.answer3.Equals("н")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer3 != null && t.answer3.Equals("н")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 17].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer4 != null && t.answer4.Equals("н")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer4 != null && t.answer4.Equals("н")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 18].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer5 != null && t.answer5.Equals("н")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer5 != null && t.answer5.Equals("н")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 19].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer6 != null && t.answer6.Equals("н")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer6 != null && t.answer6.Equals("н")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 20].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer7 != null && t.answer7.Equals("н")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer7 != null && t.answer7.Equals("н")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 21].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer8 != null && t.answer8.Equals("н")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer8 != null && t.answer8.Equals("н")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 22].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer9 != null && t.answer9.Equals("н")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer9 != null && t.answer9.Equals("н")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
                workSheet.Cells[rows, 23].Value = (st.Count(p => p.Results.Count(t => t.number_verification == 2 && t.answer10 != null && t.answer10.Equals("н")) != 0) + (st.Count(p => p.Results.Count(t => t.number_verification == 1 && t.answer10 != null && t.answer10.Equals("н")) != 0 && p.Results.Count(t => t.number_verification == 2) == 0)));
            }
            var path = Path.Combine(Server.MapPath("~/Content/files/"), "result_" + subject + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + path);
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
            FileInfo fi = new FileInfo(path);
            long sz = fi.Length;
            Response.ClearContent();
            Response.ContentType = Path.GetExtension(path);
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename = {0}", System.IO.Path.GetFileName(path)));
            Response.AddHeader("Content-Length", sz.ToString("F0"));
            Response.TransmitFile(path);
            Response.End();
            System.IO.File.Delete(path);

            return Redirect("Report/Report");
        }
        /* Итоговые результаты проверки РПК */

        /* Результаты проверок в зависимости от типа (type=1 - рпк, type=2 - апелляция, type=3 - опк) */
        public ActionResult ExportData(int subject, int type)
        {
            int rows = 3, cols = 1;
            Subjects_Numbers s = db.Subjects_Numbers.Find(subject);
            int user_id = Convert.ToInt32(Session["user"]);
            Users user = db.Users.Find(user_id);
            List<Students> st = new List<Students>();
            List<Results> res = db.Results.Where(p => p.number_verification == type && p.Students.Classes.id_subject_number == subject).ToList();
            if (user.id_role == 1)
            {
                st = db.Students.Where(p => p.Classes.id_subject_number == subject && p.cipher != null && p.scan != null && p.Results.Count(t=>t.number_verification==type) >= 1).ToList();
            }
            else if (user.id_role == 7)
            {
                Area ac = db.Area.FirstOrDefault(p => p.id_coordinator == user.id);
                st = db.Students.Where(p => p.Classes.Schools.Districts.id_area == ac.id && p.Classes.id_subject_number == subject && p.cipher != null && p.scan != null && p.Results.Count(t => t.number_verification == type) >= 1).ToList();
            }
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Результаты_" + s.subject);
            workSheet.Cells[1, 1].Value = "№ п/п";
            workSheet.Cells[1, 2].Value = "Название региона";
            workSheet.Cells[1, 3].Value = "Название района";
            workSheet.Cells[1, 4].Value = "Название учреждения образования";
            workSheet.Cells[1, 5].Value = "Код учреждения образования";
            workSheet.Cells[1, 6].Value = "Место расположения учреждения образования (г/с)";
            workSheet.Cells[1, 7].Value = "Вид учреждения образования";
            workSheet.Cells[1, 8].Value = "ФИО учащегося";
            workSheet.Cells[1, 9].Value = "Пол учащегося";
            workSheet.Cells[1, 10].Value = "Уровень";
            workSheet.Cells[1, 11].Value = "Класс (с указанием буквы)";
            workSheet.Cells[1, 12].Value = "Шифр работы";
            workSheet.Cells[1, 13].Value = "Вариант РКР";
            workSheet.Cells[1, 14].Value = "Количество баллов, полученных учащимися за задания";
            workSheet.Cells[1, 25].Value = "Ошибки";
            workSheet.Cells[1, 34].Value = "Суммарный балл за РКР (автоматизированная система перепроверяет корректность ввода суммы баллов за РКР)";
            workSheet.Cells[1, 35].Value = "Отметка за РКР (выставляется на основании суммарного балла за РКР)";
            workSheet.Cells[1, 36].Value = "Уровень усвоения учебного материала по результатам РКР (выставляется на основании суммарного балла)";
            workSheet.Cells[1, 37].Value = "Результаты итоговой аттестации по учебному предмету за 2017/18 уч. г.(отметка)";
            workSheet.Cells[1, 38].Value = "Уровень усвоения учебного материала по результатам итоговой аттестации за 2017/18 уч. г.";
            workSheet.Cells[1, 39].Value = "Результаты промежуточной аттестации за ׀ четверть 2018/19 уч. г. (отметка)";
            workSheet.Cells[1, 40].Value = "Уровень усвоения учебного материала по результатам промежуточной аттестации за ׀ четверть 2018/19 уч. г.";
            workSheet.Cells[1, 41].Value = "Результаты промежуточной аттестации за ׀׀ четверть 2018/19 уч. г. (отметка)";
            workSheet.Cells[1, 42].Value = "Уровень усвоения учебного материала по результатам итоговой аттестации за ׀׀ четверть 2018/19 уч. г.";
            workSheet.Cells[1, 43].Value = "Проверяющий. Email";
            workSheet.Cells[1, 44].Value = "Проверяющий. Область";
            if (type < 3)
            {
                workSheet.Cells[1, 45].Value = "Проверяющий. Район";
            }
            workSheet.Cells[2, 14].Value = "№ 1";
            workSheet.Cells[2, 15].Value = "№ 2";
            workSheet.Cells[2, 16].Value = "№ 3";
            workSheet.Cells[2, 17].Value = "№ 4";
            workSheet.Cells[2, 18].Value = "№ 5";
            workSheet.Cells[2, 19].Value = "№ 6";
            workSheet.Cells[2, 20].Value = "№ 7";
            workSheet.Cells[2, 21].Value = "№ 8";
            workSheet.Cells[2, 22].Value = "№ 9";
            workSheet.Cells[2, 23].Value = "№ 10";
            workSheet.Cells[2, 24].Value = "№ 1";
            workSheet.Cells[2, 25].Value = "№ 2";
            workSheet.Cells[2, 26].Value = "№ 3";
            workSheet.Cells[2, 27].Value = "№ 4";
            workSheet.Cells[2, 28].Value = "№ 5";
            workSheet.Cells[2, 29].Value = "№ 6";
            workSheet.Cells[2, 30].Value = "№ 7";
            workSheet.Cells[2, 31].Value = "№ 8";
            workSheet.Cells[2, 32].Value = "№ 9";
            workSheet.Cells[2, 33].Value = "№ 10";
            for (int i = 0; i < st.Count(); ++i)
            {
                workSheet.Cells[rows, cols++].Value = (i + 1);
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.Districts.Area.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.Districts.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.code;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.Places.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.Types_edu.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).surname + " " + st.ElementAt(i).name + " " + st.ElementAt(i).patronomic;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Genders.name;
                workSheet.Cells[rows, cols++].Value = (Convert.ToBoolean(st.ElementAt(i).level_edu)) ? "Повышенный" : "Базовый";
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Classes.letter != null) ? (st.ElementAt(i).Classes.Subjects_Numbers.number + " " + st.ElementAt(i).Classes.letter) : st.ElementAt(i).Classes.Subjects_Numbers.number;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).cipher;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).getVariant();
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).answer1;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).answer2;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).answer3;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).answer4;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).answer5;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).answer6;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).answer7;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).answer8;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).answer9;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).answer10;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).errors1;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).errors2;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).errors3;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).errors4;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).errors5;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).errors6;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).errors7;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).errors8;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).errors9;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).errors10;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).SumOfScores;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).Mark;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).Level;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).mark_year;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).getLevel(st.ElementAt(i).mark_year);
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).mark_1semestr;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).getLevel(st.ElementAt(i).mark_1semestr);
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).mark_2semestr;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).getLevel(st.ElementAt(i).mark_2semestr);
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault().Users.email;
                workSheet.Cells[rows, cols++].Value = (type<3) ? st.ElementAt(i).Results.FirstOrDefault(p=>p.number_verification==type).Users.Districts_Check.First().Districts.Area.name : st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).Users.Areas_Check.First().Area.name;
                if (type < 3)
                {
                    workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == type).Users.Districts_Check.First().Districts.name;
                }
                rows++;
                cols = 1;
            }
            if (user.id_role == 1)
            {
                rows += 5;
                workSheet.Cells[rows, 12].Value = "количество учащихся";
                workSheet.Cells[rows, 13].Value = "набравших максимальный балл";
                workSheet.Cells[rows, 14].Value = res.Count(p => p.answer1 != null && p.answer1.Equals("1"));
                workSheet.Cells[rows, 15].Value = (subject != 2) ? res.Count(p => p.answer2 != null && p.answer2.Equals("1")) : res.Count(p => p.answer2 != null && p.answer2.Equals("2"));
                workSheet.Cells[rows, 16].Value = (subject != 2) ? res.Count(p => p.answer3 != null && p.answer3.Equals("2")) : 0;
                workSheet.Cells[rows, 17].Value = (subject != 2) ? res.Count(p => p.answer4 != null && p.answer4.Equals("2")) : res.Count(p => p.answer4 != null && p.answer4.Equals("3"));
                workSheet.Cells[rows, 18].Value = res.Count(p => p.answer5 != null && p.answer5.Equals("3"));
                workSheet.Cells[rows, 19].Value = res.Count(p => p.answer6 != null && p.answer6.Equals("3"));
                workSheet.Cells[rows, 20].Value = res.Count(p => p.answer7 != null && p.answer7.Equals("4"));
                workSheet.Cells[rows, 21].Value = res.Count(p => p.answer8 != null && p.answer8.Equals("4"));
                workSheet.Cells[rows, 22].Value = res.Count(p => p.answer9 != null && p.answer9.Equals("5"));
                workSheet.Cells[rows++, 23].Value = res.Count(p => p.answer10 != null && p.answer10.Equals("5"));
                workSheet.Cells[rows, 13].Value = "не приступивших к выполнению";
                workSheet.Cells[rows, 14].Value = res.Count(p => p.answer1 != null && p.answer1.Equals("н"));
                workSheet.Cells[rows, 15].Value = res.Count(p => p.answer2 != null && p.answer2.Equals("н"));
                workSheet.Cells[rows, 16].Value = res.Count(p => p.answer3 != null && p.answer3.Equals("н"));
                workSheet.Cells[rows, 17].Value = res.Count(p => p.answer4 != null && p.answer4.Equals("н"));
                workSheet.Cells[rows, 18].Value = res.Count(p => p.answer5 != null && p.answer5.Equals("н"));
                workSheet.Cells[rows, 19].Value = res.Count(p => p.answer6 != null && p.answer6.Equals("н"));
                workSheet.Cells[rows, 20].Value = res.Count(p => p.answer7 != null && p.answer7.Equals("н"));
                workSheet.Cells[rows, 21].Value = res.Count(p => p.answer8 != null && p.answer8.Equals("н"));
                workSheet.Cells[rows, 22].Value = res.Count(p => p.answer9 != null && p.answer9.Equals("н"));
                workSheet.Cells[rows, 23].Value = res.Count(p => p.answer10 != null && p.answer10.Equals("н"));
            }
            var path = Path.Combine(Server.MapPath("~/Content/files/"), "result_" + subject + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + path);
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
            FileInfo fi = new FileInfo(path);
            long sz = fi.Length;
            Response.ClearContent();
            Response.ContentType = Path.GetExtension(path);
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename = {0}", System.IO.Path.GetFileName(path)));
            Response.AddHeader("Content-Length", sz.ToString("F0"));
            Response.TransmitFile(path);
            Response.End();
            System.IO.File.Delete(path);

            return Redirect("Report/Report");
        }
        /* Результаты проверок в зависимости от типа (end)*/


        /* Отчет сравнения РПК и ОПК */
        public ActionResult CompareResult(int subject)
        {
            int rows = 3, cols = 1;
            Subjects_Numbers s = db.Subjects_Numbers.Find(subject); //предмет
            int user_id = Convert.ToInt32(Session["user"]); //Идентификатор пользователя
            Users user = db.Users.Find(user_id); //Данные текущего пользователя
            List<Students> st = new List<Students>(); //Список учащихся
            if (user.id_role == 1) //Если информационный администратор
            {
                st = db.Students.Where(p => p.Classes.id_subject_number == subject && p.cipher != null && p.scan != null && p.Results.Count(t => t.number_verification == 3) >= 1).ToList(); //Список всех учащихся, сдающих данный предмет и работы которых были отправдены на областную проверку
            }
            else if (user.id_role == 7) //Если региональный координатор
            {
                Area ac = db.Area.FirstOrDefault(p => p.id_coordinator == user.id); //Область, из которой РК
                st = db.Students.Where(p => p.Classes.Schools.Districts.id_area == ac.id && p.Classes.id_subject_number == subject && p.cipher != null && p.scan != null && p.Results.Count(t => t.number_verification == 3) >= 1).ToList(); //Список учащихся из данной области, сдающих заданны предмет и работы которых были отправлены на областную проверку
            }
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Сравнение_результатов_" + s.subject); //Создание листа в .excel

            //Пункты, по которым собирается информация
            workSheet.Cells[1, 1].Value = "№ п/п";
            workSheet.Cells[1, 2].Value = "Название региона";
            workSheet.Cells[1, 3].Value = "Название района";
            workSheet.Cells[1, 4].Value = "Название учреждения образования";
            workSheet.Cells[1, 5].Value = "Код учреждения образования";
            workSheet.Cells[1, 6].Value = "Место расположения учреждения образования (г/с)";
            workSheet.Cells[1, 7].Value = "Вид учреждения образования";
            workSheet.Cells[1, 8].Value = "ФИО учащегося";
            workSheet.Cells[1, 9].Value = "Пол учащегося";
            workSheet.Cells[1, 10].Value = "Уровень";
            workSheet.Cells[1, 11].Value = "Класс (с указанием буквы)";
            workSheet.Cells[1, 12].Value = "Шифр работы";
            workSheet.Cells[1, 13].Value = "Вариант РКР";
            workSheet.Cells[1, 14].Value = "Количество баллов, полученных учащимися за задания";
            workSheet.Cells[1, 34].Value = "Ошибки";
            workSheet.Cells[1, 54].Value = "Суммарный балл за РКР РПК";
            workSheet.Cells[1, 55].Value = "Суммарный балл за РКР ОПК";
            workSheet.Cells[1, 56].Value = "Отметка за РКР РПК";
            workSheet.Cells[1, 57].Value = "Отметка за РКР ОПК";
            workSheet.Cells[1, 58].Value = "Уровень усвоения учебного материала по результатам РКР РПК";
            workSheet.Cells[1, 59].Value = "Уровень усвоения учебного материала по результатам РКР ОПК";
            workSheet.Cells[1, 60].Value = "Результаты итоговой аттестации по учебному предмету за 2017/18 уч. г.(отметка)";
            workSheet.Cells[1, 61].Value = "Уровень усвоения учебного материала по результатам итоговой аттестации за 2017/18 уч. г.";
            workSheet.Cells[1, 62].Value = "Результаты промежуточной аттестации за ׀ четверть 2018/19 уч. г. (отметка)";
            workSheet.Cells[1, 63].Value = "Уровень усвоения учебного материала по результатам промежуточной аттестации за ׀ четверть 2018/19 уч. г.";
            workSheet.Cells[1, 64].Value = "Результаты промежуточной аттестации за ׀׀ четверть 2018/19 уч. г. (отметка)";
            workSheet.Cells[1, 65].Value = "Уровень усвоения учебного материала по результатам итоговой аттестации за ׀׀ четверть 2018/19 уч. г.";
            workSheet.Cells[1, 66].Value = "Проверяющий ОПК. Email";
            workSheet.Cells[1, 67].Value = "Проверяющий ОПК. Область";
            workSheet.Cells[1, 68].Value = "Проверяющий РПК. Email";
            workSheet.Cells[1, 69].Value = "Проверяющий РПК. Район";
            workSheet.Cells[2, 14].Value = "№ 1 РПК";
            workSheet.Cells[2, 15].Value = "№ 1 ОПК";
            workSheet.Cells[2, 16].Value = "№ 2 РПК";
            workSheet.Cells[2, 17].Value = "№ 2 ОПК";
            workSheet.Cells[2, 18].Value = "№ 3 РПК";
            workSheet.Cells[2, 19].Value = "№ 3 ОПК";
            workSheet.Cells[2, 20].Value = "№ 4 РПК";
            workSheet.Cells[2, 21].Value = "№ 4 ОПК";
            workSheet.Cells[2, 22].Value = "№ 5 РПК";
            workSheet.Cells[2, 23].Value = "№ 5 ОПК";
            workSheet.Cells[2, 24].Value = "№ 6 РПК";
            workSheet.Cells[2, 25].Value = "№ 6 ОПК";
            workSheet.Cells[2, 26].Value = "№ 7 РПК";
            workSheet.Cells[2, 27].Value = "№ 7 ОПК";
            workSheet.Cells[2, 28].Value = "№ 8 РПК";
            workSheet.Cells[2, 29].Value = "№ 8 ОПК";
            workSheet.Cells[2, 30].Value = "№ 9 РПК";
            workSheet.Cells[2, 31].Value = "№ 9 ОПК";
            workSheet.Cells[2, 32].Value = "№ 10 РПК";
            workSheet.Cells[2, 33].Value = "№ 10 ОПК";
            workSheet.Cells[2, 34].Value = "№ 1 РПК";
            workSheet.Cells[2, 35].Value = "№ 1 ОПК";
            workSheet.Cells[2, 36].Value = "№ 2 РПК";
            workSheet.Cells[2, 37].Value = "№ 2 ОПК";
            workSheet.Cells[2, 38].Value = "№ 3 РПК";
            workSheet.Cells[2, 39].Value = "№ 3 ОПК";
            workSheet.Cells[2, 40].Value = "№ 4 РПК";
            workSheet.Cells[2, 41].Value = "№ 4 ОПК";
            workSheet.Cells[2, 42].Value = "№ 5 РПК";
            workSheet.Cells[2, 43].Value = "№ 5 ОПК";
            workSheet.Cells[2, 44].Value = "№ 6 РПК";
            workSheet.Cells[2, 45].Value = "№ 6 ОПК";
            workSheet.Cells[2, 46].Value = "№ 7 РПК";
            workSheet.Cells[2, 47].Value = "№ 7 ОПК";
            workSheet.Cells[2, 48].Value = "№ 8 РПК";
            workSheet.Cells[2, 49].Value = "№ 8 ОПК";
            workSheet.Cells[2, 50].Value = "№ 9 РПК";
            workSheet.Cells[2, 51].Value = "№ 9 ОПК";
            workSheet.Cells[2, 52].Value = "№ 10 РПК";
            workSheet.Cells[2, 53].Value = "№ 10 ОПК";
            //Пункты, по которым собирается информация

            //Запись нужной информации в .excel
            for (int i = 0; i < st.Count(); ++i)
            {
                workSheet.Cells[rows, cols++].Value = (i + 1);
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.Districts.Area.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.Districts.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.code;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.Places.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Classes.Schools.Types_edu.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).surname + " " + st.ElementAt(i).name + " " + st.ElementAt(i).patronomic;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Genders.name;
                workSheet.Cells[rows, cols++].Value = (Convert.ToBoolean(st.ElementAt(i).level_edu)) ? "Повышенный" : "Базовый" ;
                workSheet.Cells[rows, cols++].Value = (st.ElementAt(i).Classes.letter != null) ? (st.ElementAt(i).Classes.Subjects_Numbers.number + " " + st.ElementAt(i).Classes.letter) : st.ElementAt(i).Classes.Subjects_Numbers.number;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).cipher;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).getVariant();
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer1;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).answer1;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer2;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).answer2;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer3;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).answer3;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer4;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).answer4;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer5;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).answer5;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer6;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).answer6;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer7;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).answer7;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer8;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).answer8;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer9;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).answer9;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).answer10;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).answer10;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors1;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).errors1;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors2;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).errors2;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors3;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).errors3;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors4;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).errors4;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors5;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).errors5;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors6;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).errors6;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors7;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).errors7;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors8;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).errors8;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors9;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).errors9;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).errors10;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).errors10;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).SumOfScores;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).SumOfScores;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).Mark;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).Mark;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 1).Level;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p => p.number_verification == 3).Level;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).mark_year;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).getLevel(st.ElementAt(i).mark_year);
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).mark_1semestr;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).getLevel(st.ElementAt(i).mark_1semestr);
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).mark_2semestr;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).getLevel(st.ElementAt(i).mark_2semestr);
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p=>p.number_verification==3).Users.email;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p=>p.number_verification==3).Users.Areas_Check.FirstOrDefault().Area.name;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p=>p.number_verification==1).Users.email;
                workSheet.Cells[rows, cols++].Value = st.ElementAt(i).Results.FirstOrDefault(p=>p.number_verification==1).Users.Districts_Check.FirstOrDefault().Districts.name;
                cols = 1;
                rows++;
            }
            //Запись нужной информации в .excel

            var path = Path.Combine(Server.MapPath("~/Content/files/"), "compare_result" + subject + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + path);
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
            FileInfo fi = new FileInfo(path);
            long sz = fi.Length;
            Response.ClearContent();
            Response.ContentType = Path.GetExtension(path);
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename = {0}", System.IO.Path.GetFileName(path)));
            Response.AddHeader("Content-Length", sz.ToString("F0"));
            Response.TransmitFile(path);
            Response.End();
            System.IO.File.Delete(path);
            return Redirect("Report/Report");
        }
        /* Отчет сравнения РПК и ОПК (end) */

        
        /* Список всех отчетов */
        public ActionResult Report()
        {
            return View();
        }
        /* Список всех отчетов (end) */

    }
}
