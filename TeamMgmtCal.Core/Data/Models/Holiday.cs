using System;
using System.Collections.Generic;
using TeamMgmtCal.Core.Data.Attributres;

namespace TeamMgmtCal.Core.Data.Models
{
    public class Holiday : DataObject<Holiday>
    {
        public Holiday(IDataContext db) : base(db){}

        [RequiredValue(ErrorMessage = "Name Is Required")]
        public string Name { get; set; }

        [RequiredValue(ErrorMessage = "Observe Date Is Required")]
        public DateTime ObserveDate { get; set; }

    }
}