using System;
using System.Collections.Generic;
using TeamMgmtCal.Core.Data.Attributres;

namespace TeamMgmtCal.Core.Data.Models
{
    public class RequestType : DataObject<RequestType>
    {
        public RequestType(IDataContext db) : base(db){}

        public int SpecificToTeamId { get; set; }
        public bool VisibleToAllTeams { get; set; }
        
        [RequiredValue(ErrorMessage = "Name Is Required")]
        public string Name { get; set; }

    }
}