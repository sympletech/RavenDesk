using System;
using System.Collections.Generic;
using System.Linq;
using TeamMgmtCal.Core.Data;
using TeamMgmtCal.Core.Data.Models;
using TeamMgmtCal.Core.Email;
using TeamMgmtCal.Core.Helpers;
using TeamMgmtCal.Core.Shared;

namespace TeamMgmtCal.Core.UserManagement
{
    public class UserWorker
    {
        public IDataContext Db { get; set; }
        public UserWorker(IDataContext db)
        {
            this.Db = db;
        }

        public AccountUser GetUser(int id)
        {
            return Db.Session.Load<AccountUser>(id);
        }
        public AccountUser GetUser(string id)
        {
            return Db.Session.Load<AccountUser>(id);
        }

        public IEnumerable<AccountUser> GetAllUsers()
        {
            return Db.Session.Query<AccountUser>().OrderBy(x=>x.FullName);
        }

        public IEnumerable<AccountUser> GetAdministrators()
        {
            return Db.Session.Query<AccountUser>()
                .Where(x => x.Admin == true)
                .OrderBy(x => x.FullName);
        }

        public DataObjectOperationResult AddRemoveAdministrator(AccountUser user, bool isAdmin)
        {
            user.Admin = isAdmin;
            return user.Save();
        }

        public IEnumerable<AttendanceHistoryEntry> ReadAttendanceEntries(AccountUser agent, DateTime startDate, DateTime endDate)
        {
            var Entries = Db.Session.Query<CalendarEntry>().ToList();

            return from x in Db.Session.Query<CalendarEntry>()
                   where x.Approved == true
                         && x.Agent.Id == agent.Id
                         && (x.StartDate >= startDate || x.EndDate >= startDate)
                         && (x.EndDate <= endDate || x.StartDate <= endDate)
                   orderby x.StartDate
                   select new AttendanceHistoryEntry
                       {
                           Reason = x.RequestType.Name,
                           StartDate = x.StartDate,
                           EndDate = x.EndDate,
                           Hours = x.Hours
                       };

        }
    
        public IEnumerable<AttendanceHourSummary> ComputeAttendanceHourSummary(AccountUser aUser, DateTime startDate, DateTime endDate)
        {
            var aEntries = this.ReadAttendanceEntries(aUser, startDate, endDate).GroupBy(x => x.Reason);
            
            var hourSummary = new List<AttendanceHourSummary>();
            foreach (var aEntry in aEntries.Where(x=>x.Any()))
            {
                var attendanceHistoryEntry = aEntry.FirstOrDefault();
                if (attendanceHistoryEntry != null)
                {
                    var aSummary = new AttendanceHourSummary
                        {
                            Name = attendanceHistoryEntry.Reason,
                            Hours = aEntry.Sum(x=>x.Hours)
                        };
                    hourSummary.Add(aSummary);
                }
            }

            return hourSummary.OrderBy(x => x.Name);

        }

        public void UpdateUserPassword(int userID, string plainTextPassword)
        {
            var user = Db.Session.Load<AccountUser>(userID);
            user.Password = EncryptionHelpers.HashPassword(plainTextPassword);
            user.Save();
        }
    
        public TeamDetail GetDefaultTeamForUser(AccountUser user)
        {
            var visibleTeams = user.GetUserRoles().VisibleTeams;
            return visibleTeams.OrderBy(x => x.Name).FirstOrDefault();
        }
    }
}
