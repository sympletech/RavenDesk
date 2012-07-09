using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TeamMgmtCal.Core.Data;
using TeamMgmtCal.Core.Data.Models;

namespace TeamMgmtCal.Core.Calendar
{
    public class CalendarDay
    {
        public IDataContext Db { get; set; }
        public TeamDetail Team { get; set; }

        public CalendarDay(IDataContext db, TeamDetail team, DateTime calDate, int? curMonth = null)
        {
            this.Db = db;
            this.Team = team;
            this.CalDate = calDate;
            this.IsCurrentMonth = CalDate.Month == curMonth;
            this.IsCurrentDay = CalDate == DateTime.Now.Date;
        }

        public bool IsCurrentDay { get; set; }
        public bool IsCurrentMonth { get; set; }

        public DateTime CalDate { get; set; }
        public string HolidayName { get; set; }

        public IEnumerable<CalendarEntry> GetCalendarEntries()
        {
            if (this.Team == null)
                return new List<CalendarEntry>();
            
            string dayOfWeek = this.CalDate.DayOfWeek.ToString();


            var mgrID = (Team.Manager != null) ? Team.Manager.Id : "";

            var entries = Db.Session.Query<CalendarEntry>()
                .Where(x =>
                        (x.Agent.Id == mgrID || x.Team.Id == this.Team.Id)
                        && (x.StartDate >= this.CalDate || this.CalDate <= x.EndDate)
                        && x.StartDate <= this.CalDate
                        && x.Approved == true
                        && x.Agent.DayOff1 != dayOfWeek
                        && x.Agent.DayOff2 != dayOfWeek
                );

            return entries;                

        }

    }
}
