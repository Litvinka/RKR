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
    
    public partial class Time_X
    {
        public int id { get; set; }
        public System.DateTime start { get; set; }
        public System.DateTime finish { get; set; }
        public int id_role { get; set; }
        public int id_subject_number { get; set; }
        public Nullable<bool> is_appeal { get; set; }
    
        public virtual Roles Roles { get; set; }
        public virtual Subjects_Numbers Subjects_Numbers { get; set; }
    }
}