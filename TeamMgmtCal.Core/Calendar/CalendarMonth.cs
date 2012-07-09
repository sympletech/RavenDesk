using System;
using System.Collections.Generic;
using TeamMgmtCal.Core.Data;
using TeamMgmtCal.Core.Data.Models;

namespace TeamMgmtCal.Core.Calendar
{
    public class CalendarMonth
    {
        public IDataContext Db { get; set; }
        public TeamDetail Team { get; set; }

        public CalendarMonth(IDataContext db, TeamDetail team, int month, int year)
        {
            this.Db = db;
            this.Team = team;

            //sanity checks
            month = (month > 13 || month < 0) ? DateTime.Now.Month : month;
            year = (year < 1970 || year > 3000) ? DateTime.Now.Year : year;

            if (month == 0)
            {
                year--;
                month = 12;
            }

            if (month == 13)
            {
                year++;
                month = 1;
            }

            this.Month = month;
            this.Year = year;
            this.PreviousMonth = this.ComputeMonthNav(month - 1, year);
            this.NextMonth = this.ComputeMonthNav(month + 1, year);
        }

        public int Month { get; set; }
        public int Year { get; set; }

        public DateTime PreviousMonth { get; set; }
        public DateTime NextMonth { get; set; }

        public DateTime ComputeMonthNav(int month, int year)
        {
            if (month > 12)
            {
                month = 1;
                year++;
            }
            if (month < 1)
            {
                month = 12;
                year--;
            }
            return new DateTime(year, month, 1);
        }

        public string MonthName
        {
            get { return this.FirstOfTheMonth.ToString("MMMM"); }
        }
        public DateTime FirstOfTheMonth
        {
            get { return new DateTime(this.Year, this.Month, 1); }
        }

        /// <summary>
        /// The 1st Sunday prior to the 1st of the curMonth if the 1st is not a Sunday
        /// </summary>
        public DateTime FirstDayToRender
        {
            get
            {
                int adjustDaysBack = 0;
                switch (FirstOfTheMonth.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        adjustDaysBack = 1;
                        break;
                    case DayOfWeek.Tuesday:
                        adjustDaysBack = 2;
                        break;
                    case DayOfWeek.Wednesday:
                        adjustDaysBack = 3;
                        break;
                    case DayOfWeek.Thursday:
                        adjustDaysBack = 4;
                        break;
                    case DayOfWeek.Friday:
                        adjustDaysBack = 5;
                        break;
                    case DayOfWeek.Saturday:
                        adjustDaysBack = 6;
                        break;
                }

                return FirstOfTheMonth.AddDays(-adjustDaysBack);
            }
        }

        private List<CalendarWeek> _weeks;
        public List<CalendarWeek> Weeks
        {
            get
            {
                if(this._weeks == null)
                {
                    this._weeks = new List<CalendarWeek>();
                    DateTime weekStart = this.FirstDayToRender;
                    for (int i = 0; i < 6; i++)
                    {
                        this._weeks.Add(new CalendarWeek(this.Db, this.Team, weekStart, Month));
                        weekStart = weekStart.AddDays(7);
                    }                    
                }
                return this._weeks;
            }
        }
    }
}