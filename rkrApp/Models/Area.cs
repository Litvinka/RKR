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

    public partial class Area
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Area()
        {
            this.Districts = new HashSet<Districts>();
            this.Areas_Check = new HashSet<Areas_Check>();
        }
    
        public int id { get; set; }
        [Display(Name = "Область:")]
        public string name { get; set; }
        public Nullable<int> id_coordinator { get; set; }
        public int allArea(int subject)
        {
            rkrDBEntities db = new rkrDBEntities();
            return db.Results.Where(p => p.Users.Areas_Check.FirstOrDefault(d => d.id_area == this.id) != null && p.number_verification == 3 && subject == p.Students.Classes.id_subject_number).Count();
        }

        public int addArea(int subject)
        {
            rkrDBEntities db = new rkrDBEntities();
            return db.Results.Where(p => p.Users.Areas_Check.FirstOrDefault(d => d.id_area == this.id) != null && p.number_verification == 3 && p.answer1 != null && p.answer2 != null && p.answer3 != null && p.answer4 != null && p.answer5 != null && p.answer6 != null && p.answer7 != null && p.answer8 != null && p.answer9 != null && p.answer10 != null && subject == p.Students.Classes.id_subject_number).Count();
        }

        //public int allItogArea(int subject)
        //{
        //    rkrDBEntities db = new rkrDBEntities();
        //    return db.Results.Where(p => p.Users.Areas_Check1.FirstOrDefault(d => d.id_area == this.id) != null && p.number_verification == 3 && subject == p.Students.Classes.id_subject_number).Count();
        //}

        //public int addItogArea(int subject)
        //{
        //    rkrDBEntities db = new rkrDBEntities();
        //    return db.Results.Where(p => p.Users.Areas_Check1.FirstOrDefault(d => d.id_area == this.id) != null && p.number_verification == 3 && p.answer1 != null && p.answer2 != null && p.answer3 != null && p.answer4 != null && p.answer5 != null && p.answer6 != null && p.answer7 != null && p.answer8 != null && p.answer9 != null && p.answer10 != null && subject == p.Students.Classes.id_subject_number).Count();
        //}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Districts> Districts { get; set; }
        public virtual Users Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Areas_Check> Areas_Check { get; set; }
    }
}
