using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Indexes;
using TeamMgmtCal.Core.Data.Attributres;

namespace TeamMgmtCal.Core.Data.Models
{
    public class TeamDetail : DataObject<TeamDetail>
    {
        //-- Constructors

        public TeamDetail(IDataContext db) : base(db){}

        internal TeamDetail() :base(new DataContext()){}


        //-- Properties

        [RequiredValue(ErrorMessage = "Name Is Required")]
        public string Name { get; set; }

        [Email]
        public string EmailDistro { get; set; }

        [UpdateOnSave]
        public AccountUser Manager { get; set; }

        [UpdateOnSave]
        public AccountUser AssistantManager { get; set; }

        [UpdateOnSave]
        public AccountUser HrAdmin { get; set; }
        
        public IEnumerable<AccountUser> TeamMembers { get; set; }
        public IEnumerable<RequestType> CalendarRequestTypes { get; set; }
        public IEnumerable<TeamShift> Shifts { get; set; }


        //-- CRUD

        public new DataObjectOperationResult Save()
        {
            try
            {
                if(string.IsNullOrEmpty(this.Id))
                {
                    InitNewTeam();
                }

                this.TeamMembers = (this.TeamMembers ?? new List<AccountUser>())
                    .OrderBy(x=>x.LastName).ThenBy(x=>x.FirstName).Distinct();

                var results = base.Save();

                SyncTeamMembership();

                return results;
            }
            catch (Exception ex)
            {
                return new DataObjectOperationResult
                    {
                        Success = false,
                        Message = ex.Message
                    };
                
            }
        }

        protected void InitNewTeam()
        {
            if (Db.Session.Query<TeamDetail>().Any(x => x.Name == this.Name))
            {
                throw new Exception("Team Name Already Exists!");
            }

            //Add Default request types to the Team
            this.CalendarRequestTypes = Db.Session.Query<RequestType>().Where(x => x.VisibleToAllTeams == true).ToList();
        
            //Create Default Shifts
            var shiftDefs = new List<Tuple<string, TimeSpan, TimeSpan>>();
            shiftDefs.Add(new Tuple<string, TimeSpan, TimeSpan>("Day Shift", TimeSpan.Parse("06:00"), TimeSpan.Parse("15:00")));
            shiftDefs.Add(new Tuple<string, TimeSpan, TimeSpan>("Swing Shift", TimeSpan.Parse("14:00"), TimeSpan.Parse("23:00")));
            shiftDefs.Add(new Tuple<string, TimeSpan, TimeSpan>("Grave Shift", TimeSpan.Parse("22:00"), TimeSpan.Parse("7:00")));
            var shifts = new List<TeamShift>();
            foreach (var teamShift in shiftDefs)
            {
                var tShift = new TeamShift(Db)
                    {
                        Name = teamShift.Item1,
                        StartTime = teamShift.Item2,
                        EndTime = teamShift.Item3,
                    };
                shifts.Add(tShift);
            }
            this.Shifts = shifts;
        }

        /// <summary>
        /// Sync The Team membership for agents on team
        /// </summary>
        protected void SyncTeamMembership()
        {
            //find any angents with this team listed as there team that are not on the team
            var usersClaimingMembership = Db.Session.Query<AccountUser>().Where(x => x.TeamId == this.Id);
            foreach (var user in usersClaimingMembership)
            {
                if(this.TeamMembers.Any(x=>x.Id == user.Id) != true)
                {
                    user.TeamId = null;
                }
            }

            foreach (var user in this.TeamMembers)
            {
                var actUser = Db.Session.Load<AccountUser>(user.Id);
                actUser.TeamId = this.Id;
            }

            Db.Session.SaveChanges();
        }



    }
}