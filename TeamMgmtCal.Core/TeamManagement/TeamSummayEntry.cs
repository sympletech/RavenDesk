using System;
using System.Collections.Generic;
using TeamMgmtCal.Core.Data.Models;

namespace TeamMgmtCal.Core.TeamManagement
{
    public class TeamSummayEntry
    {
        public AccountUser Agent { get; set; }

        public Dictionary<String, Double> HoursByReqestType { get; set; }
        public double TotalHours { get; set; }
    }
}
