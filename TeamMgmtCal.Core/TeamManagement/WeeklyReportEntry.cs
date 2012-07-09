using System;
using System.Collections.Generic;
using TeamMgmtCal.Core.Data.Models;

namespace TeamMgmtCal.Core.TeamManagement
{
    public class WeeklyReportEntry
    {
        public DateTime ReportDay { get; set; }
        public IEnumerable<CalendarEntry> AgentsOff { get; set; }
    }
}
