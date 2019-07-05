//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace rkrApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public partial class Districts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Districts()
        {
            this.Districts_Check = new HashSet<Districts_Check>();
            this.Schools = new HashSet<Schools>();
        }
    
        public int id { get; set; }
        [Display(Name = "Район:")]
        public string name { get; set; }
        public int id_area { get; set; }
        public int allCipher(int user, int subject)
        {
            rkrDBEntities db = new rkrDBEntities();
            int role = db.Users.Find(user).id_role;
            if (role == 7)
            {
                Area ac = db.Area.FirstOrDefault(d => d.id_coordinator == user);
                if (ac == null) { return 0; }
            }
            return db.Students.Where(p => p.Classes.id_subject_number == subject && p.Classes.Schools.id_district == this.id).Count();
        }

        public int addCipher(int user, int subject)
        {
            rkrDBEntities db = new rkrDBEntities();
            int role = db.Users.Find(user).id_role;
            if (role == 7)
            {
                Area ac = db.Area.FirstOrDefault(d => d.id_coordinator == user);
                if (ac == null) { return 0; }
            }
            return db.Students.Where(p => p.cipher != null && p.Classes.id_subject_number == subject && p.Classes.Schools.id_district == this.id).Count();
        }

        public int allScans(int user, int subject)
        {
            rkrDBEntities db = new rkrDBEntities();
            int role = db.Users.Find(user).id_role;
            if (role == 7)
            {
                Area ac = db.Area.FirstOrDefault(d => d.id_coordinator == user);
                if (ac == null) { return 0; }
            }
            return db.Students.Where(p => p.cipher != null && p.Classes.id_subject_number == subject && p.Classes.Schools.id_district == this.id).Count();
        }

        public int addScans(int user, int subject)
        {
            rkrDBEntities db = new rkrDBEntities();
            int role = db.Users.Find(user).id_role;
            if (role == 7)
            {
                Area ac = db.Area.FirstOrDefault(d => d.id_coordinator == user);
                if (ac == null) { return 0; }
            }
            return db.Students.Where(p => p.cipher != null && p.scan != null && p.Classes.id_subject_number == subject && p.Classes.Schools.id_district == this.id).Count();
        }

        public int allDistrict(int user, int subject)
        {
            rkrDBEntities db = new rkrDBEntities();
            int role = db.Users.Find(user).id_role;
            if (role == 7)
            {
                Area ac = db.Area.FirstOrDefault(d => d.id_coordinator == user);
                if (ac == null) { return 0; }
            }
            return db.Results.Where(p => p.number_verification == 1 && p.Students.Classes.id_subject_number == subject && p.Users.Districts_Check.FirstOrDefault(k => k.id_district == this.id) != null).Count();
        }

        public int addDistrict(int user, int subject)
        {
            rkrDBEntities db = new rkrDBEntities();
            int role = db.Users.Find(user).id_role;
            if (role == 7)
            {
                Area ac = db.Area.FirstOrDefault(d => d.id_coordinator == user);
                if (ac == null) { return 0; }
            }
            return db.Results.Where(p => p.number_verification == 1 && p.Students.Classes.id_subject_number == subject && p.answer1 != null && p.Users.Districts_Check.FirstOrDefault(k=>k.id_district == this.id)!=null).Count();
        }


        public int allDistrict2(int user, int subject)
        {
            rkrDBEntities db = new rkrDBEntities();
            int role = db.Users.Find(user).id_role;
            if (role == 7)
            {
                Area ac = db.Area.FirstOrDefault(d => d.id_coordinator == user);
                if (ac == null) { return 0; }
            }
            return db.Results.Where(p => p.number_verification == 2 && p.Students.Classes.id_subject_number == subject && p.Users.Districts_Check.FirstOrDefault(k => k.id_district == this.id) != null).Count();
        }

        public int addDistrict2(int user, int subject)
        {
            rkrDBEntities db = new rkrDBEntities();
            int role = db.Users.Find(user).id_role;
            if (role == 7)
            {
                Area ac = db.Area.FirstOrDefault(d => d.id_coordinator == user);
                if (ac == null) { return 0; }
            }
            return db.Results.Where(p => p.number_verification == 2 && p.Students.Classes.id_subject_number == subject && p.answer1 != null && p.Users.Districts_Check.FirstOrDefault(k => k.id_district == this.id) != null).Count();
        }


        public int allArea(int user, int subject)
        {
            rkrDBEntities db = new rkrDBEntities();
            Areas_Check ac = db.Area.FirstOrDefault(d => d.id_coordinator == user).Areas_Check.FirstOrDefault(p => p.id_subject_number == subject);
            if (ac == null) { return 0; }
            return db.Results.Where(p => p.number_verification == 3 && ac.id_user_check == p.id_user && subject == p.Students.Classes.id_subject_number).Count();
        }

        public int addArea(int user, int subject)
        {
            rkrDBEntities db = new rkrDBEntities();
            Areas_Check ac = db.Area.FirstOrDefault(d => d.id_coordinator == user).Areas_Check.FirstOrDefault(p => p.id_subject_number == subject);
            if (ac == null) { return 0; }
            return db.Results.Where(p => p.number_verification == 3 && p.answer1 != null && ac.id_user_check == p.id_user && subject == p.Students.Classes.id_subject_number).Count();
        }

        //public int allItogArea(int user, int subject)
        //{
        //    rkrDBEntities db = new rkrDBEntities();
        //    Areas_Check ac = db.Area.FirstOrDefault(d => d.id_coordinator == user).Areas_Check.FirstOrDefault(p => p.id_subject_number == subject);
        //    if (ac == null) { return 0; }
        //    return db.Results.Where(p => p.number_verification == 3 && ac.id_user_check_itog == p.id_user && subject == p.Students.Classes.id_subject_number).Count();
        //}

        //public int addItogArea(int user, int subject)
        //{
        //    rkrDBEntities db = new rkrDBEntities();
        //    Areas_Check ac = db.Area.FirstOrDefault(d => d.id_coordinator == user).Areas_Check.FirstOrDefault(p => p.id_subject_number == subject);
        //    if (ac == null) { return 0; }
        //    return db.Results.Where(p => p.number_verification == 3 && p.answer1 != null && ac.id_user_check_itog == p.id_user && subject == p.Students.Classes.id_subject_number).Count();
        //}

        public virtual Area Area { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Districts_Check> Districts_Check { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Schools> Schools { get; set; }
    }
}
