using System;
using System.Collections.Generic;
using TeamMgmtCal.Core.Data;
using TeamMgmtCal.Core.Data.Models;

namespace TeamMgmtCal.Core.Calendar
{
    public class CalendarWeek
    {
        public IDataContext Db { get; set; }
        public TeamDetail Team { get; set; }

        public CalendarWeek(IDataContext db, TeamDetail team, DateTime startDate, int? month = null)
        {
            this.Db = db;
            this.Team = team;

            this.StartDate = startDate;
            
            this.EndDate = startDate.AddDays(6);
            this.Month = month ?? 0;
        }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int Month { get; set; }

        private List<CalendarDay> _calendarDays;
        public List<CalendarDay> CalendarDays
        {
            get
            {
                if (this._calendarDays == null)
                {
                    this._calendarDays = new List<CalendarDay>();
                    for (int i = 0; i <= 6; i++)
                    {
                        this._calendarDays.Add(new CalendarDay(this.Db, this.Team, this.StartDate.AddDays(i), Month));
                    }
                }
                return this._calendarDays;
            }
        }
    }
}
