using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList
{
    class DateWeek
    {
        public string monday = "";
        public string tuesday = "";
        public string wednesday = "";
        public string thursday = "";
        public string friday = "";
        public string saturday = "";
        public string sunday = "";
        public string year = "";
        public void FindDateOfWeek()
        {
            DateTime date = DateTime.Now;
            while (date.DayOfWeek != System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(-1);
            monday = date.ToString("dd.MM");
            tuesday =  date.AddDays(+1).ToString("dd.MM");
            wednesday = date.AddDays(+2).ToString("dd.MM");
            thursday =  date.AddDays(+3).ToString("dd.MM");
            friday = date.AddDays(+4).ToString("dd.MM");
            saturday =  date.AddDays(+5).ToString("dd.MM");
            sunday = date.AddDays(+6).ToString("dd.MM");

            year = date.ToString("yyyy");
        }
    }
}
