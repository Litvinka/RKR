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
    
    public partial class Schools_Observers
    {
        public int id { get; set; }
        public int id_school { get; set; }
        public int id_observer { get; set; }
    
        public virtual Schools Schools { get; set; }
        public virtual Users Users { get; set; }
    }
}
