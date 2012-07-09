using System.Collections.Generic;
using System.Linq;
using TeamMgmtCal.Core.Data.Models;

namespace TeamMgmtCal.Core.Security
{
    public class UserRole
    {
        public int UserID { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsManager { get; set; }
        public IEnumerable<TeamDetail> TeamsManaged { get; set; }

        public bool IsHRManager { get; set; }
        public IQueryable<TeamDetail> VisibleTeams { get; set; } 
    }
}
