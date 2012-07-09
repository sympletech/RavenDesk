using System;

namespace TeamMgmtCal.Core.UserManagement
{
    public class AttendanceHistoryEntry
    {
        public string Reason { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Hours { get; set; }
    }
}
