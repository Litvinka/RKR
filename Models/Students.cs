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

    public partial class Students
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Students()
        {
            this.Results = new HashSet<Results>();
        }

        [Display(Name = "Учащийся:")]
        public int id { get; set; }

        [Display(Name = "Шифр:")]
        public string cipher { get; set; }

        //[RegularExpression(@"/W", ErrorMessage = "Используйте только кирилицу")]
        [Required(ErrorMessage = "Требуется поле Фамилия")]
        [Display(Name = "Фамилия:")]
        public string surname { get; set; }

        [Required(ErrorMessage = "Требуется поле Имя")]
        [Display(Name = "Имя:")]
        public string name { get; set; }

        [Required(ErrorMessage = "Требуется поле Отчество")]
        [Display(Name = "Отчество:")]
        public string patronomic { get; set; }

        [Required(ErrorMessage = "Требуется поле")]
        [Display(Name = "Годовая отметка:")]
        public byte mark_year { get; set; }

        [Required(ErrorMessage = "Требуется поле I четверть")]
        [Display(Name = "I четверть:")]
        public byte mark_1semestr { get; set; }

        [Required(ErrorMessage = "Требуется поле II четверть")]
        [Display(Name = "II четверть:")]
        public byte mark_2semestr { get; set; }

        [Required(ErrorMessage = "Требуется поле Номер по журналу")]
        [Display(Name = "Номер по журналу:")]
        public byte number_in_the_list { get; set; }

        [Display(Name = "Вариант:")]
        public Nullable<byte> variant { get; set; }

        [Display(Name = "Бланк ответов:")]
        public string scan { get; set; }

        [Display(Name = "Класс:")]
        public int id_classs { get; set; }

        [Display(Name = "Пол:")]
        [Required(ErrorMessage = "Пол")]
        public int id_gender { get; set; }
        public Nullable<System.DateTime> last_change { get; set; }
        public string allname
        {
            get
            {
                return number_in_the_list + ". " + surname + " " + name + " " + patronomic;
            }
        }

        public string date
        {
            get
            {
                string t=Convert.ToDateTime(this.Classes.Subjects_Numbers.start).ToString("dd");
                return t;
            }
        }

        public string getVariant()
        {
            return this.cipher.Substring(this.cipher.Length - 1);
        }

        public string getNumber()
        {
            return this.cipher.Substring(this.cipher.Length - 4, 3);
        }

        public string getLevel(int mark)
        {
            string _Level = "";
            if (mark == 1 || mark == 2)
                _Level = "первый уровень(низкий)";
            if (mark == 3 || mark == 4)
                _Level = "второй уровень(удовлетворительный)";
            if (mark == 5 || mark == 6)
                _Level = "третий уровень(средний)";
            if (mark == 7 || mark == 8)
                _Level = "четвёртый уровень(достаточный)";
            if (mark == 9 || mark == 10)
                _Level = "пятый уровень(высокий)";
            return _Level;
        }


        //сдана ли контрольная(добавлен ли скан)
        public bool IsPassed()
        {
            if (this.scan != null)
                return true;
            else return false;
        }
        //есть ли разница у районной и областной комиссии
        public bool IsHaveDifference()
        {
            if (this.Results.Where(s => s.number_verification == 1).First().answer1 != this.Results.Where(s => s.number_verification == 2).First().answer1 ||
                        this.Results.Where(s => s.number_verification == 1).First().answer2 != this.Results.Where(s => s.number_verification == 2).First().answer2 ||
                        this.Results.Where(s => s.number_verification == 1).First().answer3 != this.Results.Where(s => s.number_verification == 2).First().answer3 ||
                        this.Results.Where(s => s.number_verification == 1).First().answer4 != this.Results.Where(s => s.number_verification == 2).First().answer4 ||
                        this.Results.Where(s => s.number_verification == 1).First().answer5 != this.Results.Where(s => s.number_verification == 2).First().answer5 ||
                        this.Results.Where(s => s.number_verification == 1).First().answer6 != this.Results.Where(s => s.number_verification == 2).First().answer6 ||
                        this.Results.Where(s => s.number_verification == 1).First().answer7 != this.Results.Where(s => s.number_verification == 2).First().answer7 ||
                        this.Results.Where(s => s.number_verification == 1).First().answer8 != this.Results.Where(s => s.number_verification == 2).First().answer8 ||
                        this.Results.Where(s => s.number_verification == 1).First().answer9 != this.Results.Where(s => s.number_verification == 2).First().answer9 ||
                        this.Results.Where(s => s.number_verification == 1).First().answer10 != this.Results.Where(s => s.number_verification == 2).First().answer10)
                return true;
            else return false;

        }

        public bool IsForAreaChecker(Areas_Check checker)
        {
            //сдан, с той области и тот предмет
            if (IsPassed() && this.Classes.Schools.Districts.id_area == checker.id_area)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsForAreaItog
        {
            //сдан, с той области и тот предмет
            get
            {
                if (this.Results.Where(i => i.number_verification == 1).Count() != 0 && this.Results.Where(i => i.number_verification == 2).Count() != 0)
                {
                    if (IsPassed() &&
                    IsHaveDifference())
                        return true;
                    else return false;
                }
                else return false;
            }
        }
        public bool IsForAreaItogChecher(Areas_Check checker)
        {
            //сдан, с той области и тот предмет
            if (this.Classes.Schools.Districts.id_area != checker.id_area &&
                checker.id_area != this.Results.Where(s => s.number_verification == 2).First().Users.Areas_Check.First().id_area)
                return true;
            else return false;
        }

        public virtual Classes Classes { get; set; }
        public virtual Genders Genders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Results> Results { get; set; }
    }
}
