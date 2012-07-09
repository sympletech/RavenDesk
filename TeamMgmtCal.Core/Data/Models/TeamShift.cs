using System;
using TeamMgmtCal.Core.Data.Attributres;
using System.Linq;
using System.Collections.Generic;

namespace TeamMgmtCal.Core.Data.Models
{
    public class TeamShift : DataObject<TeamShift>
    {
        public TeamShift(IDataContext db) : base(db){}

        [RequiredValue(ErrorMessage = "Name Is Required")]
        public string Name { get; set; }

        [RequiredValue(ErrorMessage = "Start Time Is Required")]
        public TimeSpan StartTime { get; set; }

        [RequiredValue(ErrorMessage = "End Time Is Required")]
        public TimeSpan EndTime { get; set; }

    }
}