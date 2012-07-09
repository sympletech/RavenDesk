using System;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using Raven.Imports.Newtonsoft.Json;
using TeamMgmtCal.Core.Data.Attributres;
using TeamMgmtCal.Core.Email;
using TeamMgmtCal.Core.Helpers;
using TeamMgmtCal.Core.Security;
using System.Collections.Generic;


namespace TeamMgmtCal.Core.Data.Models
{
    public class AccountUser : DataObject<AccountUser>
    {
        //-- Constructors
        
        public AccountUser(IDataContext db) : base(db){}


        //-- Properties

        [RequiredValue(ErrorMessage = "Username Is Required")]
        public string Username { get; set; }

        [JsonIgnore]
        public string NewPassword { get; set; }
        public string Password { get; set; }

        [RequiredValue(ErrorMessage = "Last Name Is Required")]
        public string LastName { get; set; }

        [RequiredValue(ErrorMessage = "First Name Is Required")]
        public string FirstName { get; set; }

        [JsonIgnore]
        public string FullName { 
            get { return string.Format("{0}, {1}", this.LastName, this.FirstName); }
        }

        public string Position { get; set; }

        [Email]
        public string Email { get; set; }

        public string PhoneOffice { get; set; }
        public string PhoneCell { get; set; }
        public string PhoneHome { get; set; }
        public string DayOff1 { get; set; }
        public string DayOff2 { get; set; }

        public string TeamId { get; set; }

        [UpdateOnSave]
        public TeamShift Shift { get; set; }

        public bool Active { get; set; }
        public bool Admin { get; set; }

        public bool MustChangePassword { get; set; }

        
        //-- Lookups


        /// <summary>
        /// Lookup The Roles of Account User
        /// </summary>
        public UserRole GetUserRoles()
        {
            return new UserRole
            {
                IsAdmin = this.Admin,
                IsManager = Db.Session.Query<TeamDetail>().Any(
                    x => x.Manager.Id == this.Id || x.AssistantManager.Id == this.Id),
                IsHRManager = Db.Session.Query<TeamDetail>().Any(x => x.HrAdmin.Id == this.Id),
                VisibleTeams = GetVisibleTeams()
            };
        }

        /// <summary>
        /// Get Teams Visible To Account User
        /// </summary>
        public IQueryable<TeamDetail> GetVisibleTeams()
        {
            return from x in Db.Session.Query<TeamDetail>()
                   where x.Manager.Id == this.Id
                         || x.AssistantManager.Id == this.Id
                         || x.HrAdmin.Id == this.Id
                         || x.TeamMembers.Any(y => y.Id == this.Id)
                   orderby x.Name
                   select x;
        }
        

        //-- CRUD

        public new DataObjectOperationResult Save()
        {
            try
            {
                if(string.IsNullOrEmpty(this.Id))
                {
                    this.InitNewAccount();
                }

                //Encrypt New Password
                if (string.IsNullOrEmpty(this.NewPassword) != true)
                {
                    this.Password = EncryptionHelpers.HashPassword(this.NewPassword);
                }

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

        /// <summary>
        /// Checks For Duplicate Accounts and sets a temp password
        /// </summary>
        protected void InitNewAccount()
        {
            //Check to ensure username and e-mail not in use
            if (Db.Session.Query<AccountUser>().Any(x => x.Username == this.Username))
            {
                throw new Exception("Username already exists");
            }

            if (Db.Session.Query<AccountUser>().Any(x => x.Email == this.Email))
            {
                throw new Exception("E-Mail already exists");
            }

            //Create a new Password if one is not set
            if (string.IsNullOrEmpty(this.NewPassword))
            {
                this.NewPassword = Guid.NewGuid().ToString();
            }

            //Send User Account Info
            var emlWorker = new EmailWorker();
            emlWorker.SendNewUserInfoEmail(this, this.NewPassword);            
        }

        /// <summary>
        /// Sync The Agent to the correct team
        /// </summary>
        protected void SyncTeamMembership()
        {
            //Remove from any teams that list agent as a member except current team
            var notMyTeams = Db.Session.Query<TeamDetail>().Where(x => x.TeamMembers.Any(y => y.Id == this.Id));
            foreach (var team in notMyTeams)
            {
                team.TeamMembers = team.TeamMembers.Where(x => x.Id != this.Id);
                team.Save();
            }

            if (string.IsNullOrEmpty(this.TeamId) != true)
            {
                var myTeam = Db.Session.Load<TeamDetail>(this.TeamId);

                List<AccountUser> tMembers = (myTeam.TeamMembers ?? new List<AccountUser>()).ToList();
                tMembers.Add(this);

                myTeam.TeamMembers = tMembers.Distinct().OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
                myTeam.Save();                
            }
        }

        public class UserTeamMapIndex : AbstractIndexCreationTask<TeamDetail, TeamDetail>
        {
            public UserTeamMapIndex()
            {
                Map = teams => from team in teams
                               select new TeamDetail
                                   {
                                       Id = team.Id,
                                       Name = team.Name
                                   };

                Reduce = results => from result in results
                                    select new TeamDetail
                                    {
                                        Id = result.Id,
                                        Name = result.Name
                                    };
            }

        }

    }
}