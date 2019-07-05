namespace rkrApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Results
    {
        public int id { get; set; }
        public int id_student { get; set; }
        [RegularExpression(@"([0][0-9,]{1,3})|[н]|[0-1]", ErrorMessage = "неверное значение")]
        [Display(Name = "№1")]
        public string answer1 { get; set; }
        [RegularExpression(@"([0][0-9,]{1,3})|[н]|[0-1]", ErrorMessage = "неверное значение")]
        [Display(Name = "№2")]
        public string answer2 { get; set; }
        [RegularExpression(@"([0-1][0-9,]{1,3})|[н]|[0-2]", ErrorMessage = "неверное значение")]
        [Display(Name = "№3")]
        public string answer3 { get; set; }
        [RegularExpression(@"([0-1][0-9,]{1,3})|[н]|[0-2]", ErrorMessage = "неверное значение")]
        [Display(Name = "№4")]
        public string answer4 { get; set; }
        [RegularExpression(@"([0-2][0-9,]{1,3})|[н]|[0-3]", ErrorMessage = "неверное значение")]
        [Display(Name = "№5")]
        public string answer5 { get; set; }
        [RegularExpression(@"([0-2][0-9,]{1,3})|[н]|[0-3]", ErrorMessage = "неверное значение")]
        [Display(Name = "№6")]
        public string answer6 { get; set; }
        [RegularExpression(@"([0-3][0-9,]{1,3})|[н]|[0-4]", ErrorMessage = "неверное значение")]
        [Display(Name = "№7")]
        public string answer7 { get; set; }
        [RegularExpression(@"([0-3][0-9,]{1,3})|[н]|[0-4]", ErrorMessage = "неверное значение")]
        [Display(Name = "№8")]
        public string answer8 { get; set; }
        [RegularExpression(@"([0-4][0-9,]{1,3})|[н]|[0-5]", ErrorMessage = "неверное значение")]
        [Display(Name = "№9")]
        public string answer9 { get; set; }
        [RegularExpression(@"([0-4][0-9,]{1,3})|[н]|[0-5]", ErrorMessage = "неверное значение")]
        [Display(Name = "№10")]
        public string answer10 { get; set; }
        public int id_user { get; set; }
        public byte number_verification { get; set; }
        public Nullable<System.DateTime> date_verification { get; set; }
        public Nullable<bool> necessarily { get; set; }
        [RegularExpression(@"([0-9]{1,}[.]{1}[0-9]{1,}[;]?){0,}", ErrorMessage = "неверное значение")]
        public string errors1 { get; set; }
        [RegularExpression(@"([0-9]{1,}[.]{1}[0-9]{1,}[;]?){0,}", ErrorMessage = "неверное значение")]
        public string errors2 { get; set; }
        [RegularExpression(@"([0-9]{1,}[.]{1}[0-9]{1,}[;]?){0,}", ErrorMessage = "неверное значение")]
        public string errors3 { get; set; }
        [RegularExpression(@"([0-9]{1,}[.]{1}[0-9]{1,}[;]?){0,}", ErrorMessage = "неверное значение")]
        public string errors4 { get; set; }
        [RegularExpression(@"([0-9]{1,}[.]{1}[0-9]{1,}[;]?){0,}", ErrorMessage = "неверное значение")]
        public string errors5 { get; set; }
        [RegularExpression(@"([0-9]{1,}[.]{1}[0-9]{1,}[;]?){0,}", ErrorMessage = "неверное значение")]
        public string errors6 { get; set; }
        [RegularExpression(@"([0-9]{1,}[.]{1}[0-9]{1,}[;]?){0,}", ErrorMessage = "неверное значение")]
        public string errors7 { get; set; }
        [RegularExpression(@"([0-9]{1,}[.]{1}[0-9]{1,}[;]?){0,}", ErrorMessage = "неверное значение")]
        public string errors8 { get; set; }
        [RegularExpression(@"([0-9]{1,}[.]{1}[0-9]{1,}[;]?){0,}", ErrorMessage = "неверное значение")]
        public string errors9 { get; set; }
        [RegularExpression(@"([0-9]{1,}[.]{1}[0-9]{1,}[;]?){0,}", ErrorMessage = "неверное значение")]
        public string errors10 { get; set; }
        [Display(Name = "Суммарный балл:")]
        public double SumOfScores
        {
            get
            {
                double _SumOfScores = 0.0;
                double res = 0;

                if (Double.TryParse(answer1, out res)) { _SumOfScores += Double.Parse(answer1); }
                if (Double.TryParse(answer2, out res)) { _SumOfScores += Double.Parse(answer2); }
                if (Double.TryParse(answer3, out res)) { _SumOfScores += Double.Parse(answer3); }
                if (Double.TryParse(answer4, out res)) { _SumOfScores += Double.Parse(answer4); }
                if (Double.TryParse(answer5, out res)) { _SumOfScores += Double.Parse(answer5); }
                if (Double.TryParse(answer6, out res)) { _SumOfScores += Double.Parse(answer6); }
                if (Double.TryParse(answer7, out res)) { _SumOfScores += Double.Parse(answer7); }
                if (Double.TryParse(answer8, out res)) { _SumOfScores += Double.Parse(answer8); }
                if (Double.TryParse(answer9, out res)) { _SumOfScores += Double.Parse(answer9); }
                if (Double.TryParse(answer10, out res)) { _SumOfScores += Double.Parse(answer10); }
                return Convert.ToDouble(_SumOfScores);
            }
        }
        [Display(Name = "Отметка:")]
        public int Mark
        {
            get
            {
                int _Mark = 0;
                int sum= Convert.ToInt32(Math.Round(SumOfScores, 0, MidpointRounding.AwayFromZero));
                if (sum == 1)
                    _Mark = 1;
                else if (sum == 2)
                    _Mark = 2;
                else if (sum >= 3 && sum <= 5)
                    _Mark = 3;
                else if (sum >= 6 && sum <= 8)
                    _Mark = 4;
                else if (sum >= 9 && sum <= 11)
                    _Mark = 5;
                else if (sum >= 12 && sum <= 14)
                    _Mark = 6;
                else if (sum >= 15 && sum <= 18)
                    _Mark = 7;
                else if (sum >= 19 && sum <= 23)
                    _Mark = 8;
                else if (sum >= 24 && sum <= 28)
                    _Mark = 9;
                else if (sum >= 29 && sum <= 30)
                    _Mark = 10;
                return _Mark;
            }
        }
        [Display(Name = "Уровень усвоения учебного материала:")]
        public string Level
        {
            get
            {
                string _Level = "";
                if (Mark == 1 || Mark == 2)
                    _Level = "первый уровень(низкий)";
                if (Mark == 3 || Mark == 4)
                    _Level = "второй уровень(удовлетворительный)";
                if (Mark == 5 || Mark == 6)
                    _Level = "третий уровень(средний)";
                if (Mark == 7 || Mark == 8)
                    _Level = "четвёртый уровень(достаточный)";
                if (Mark == 9 || Mark == 10)
                    _Level = "пятый уровень(высокий)";
                return _Level;
            }
        }

        public int getVariant()
        {
            return Convert.ToInt32(this.Students.cipher.Substring(this.Students.cipher.Length - 1));
        }

        public virtual Students Students { get; set; }
        public virtual Users Users { get; set; }
    }
}
