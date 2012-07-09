using System;
using System.Linq;
using TeamMgmtCal.Core.Data.Attributres;
using TeamMgmtCal.Core.Email;

namespace TeamMgmtCal.Core.Data.Models
{
    public class CalendarEntry : DataObject<CalendarEntry>
    {
        //-- Constructors

        public CalendarEntry(IDataContext db) : base(db){}


        //-- Properties

        [UpdateOnSave]
        [RequiredValue(ErrorMessage = "Agent Is Required")]
        public AccountUser Agent { get; set; }

        public TeamDetail Team { get; set; }

        [UpdateOnSave]
        public Holiday Holiday { get; set; }

        [UpdateOnSave]
        [RequiredValue(ErrorMessage = "Request Reason Is Required")]
        public RequestType RequestType { get; set; }

        public string RequestNotes { get; set; }

        [RequiredValue(ErrorMessage = "Start Date Is Required")]
        public DateTime StartDate { get; set; }

        [RequiredValue(ErrorMessage = "End Date Is Required")]
        public DateTime EndDate { get; set; }

        [RequiredValue(ErrorMessage = "Hours Are Required")]
        public double Hours { get; set; }
        
        [UpdateOnSave]
        public AccountUser RequestedBy { get; set; }
        public DateTime RequestedOn { get; set; }

        [UpdateOnSave]
        public AccountUser ResponseBy { get; set; }
        public DateTime ResponseOn { get; set; }

        public string ResponseNotes { get; set; }
        public bool? Approved { get; set; }

        //-- Methods

        /// <summary>
        /// Returns the number of hours the submitted request will span - excluding normal days off
        /// </summary>
        public int ComputeRequestHours()
        {
            DateTime cDate = this.StartDate;
            int reqHours = 0;
            while (cDate.Date <= this.EndDate)
            {
                if ((cDate.DayOfWeek.ToString() != this.Agent.DayOff1) && (cDate.DayOfWeek.ToString() != this.Agent.DayOff2))
                {
                    reqHours += 8;
                }
                cDate = cDate.AddDays(1);
            }

            return reqHours;
        }

        /// <summary>
        /// Submits a new request to be reviewed by team manager
        /// </summary>
        public DataObjectOperationResult SubmitNewRequest()
        {
            //Check to ensure Request dosent exist
            var exists = Db.Session.Query<CalendarEntry>()
                .Any(x =>
                     x.Agent.Id == this.Agent.Id
                     && x.StartDate == this.StartDate
                     && x.EndDate == this.EndDate
                     && x.Approved == this.Approved);

            if (exists)
            {
                var eMsg = string.Format("The Request for {0} Starting on {1} Ending on {2} is a duplicate request.",
                    this.RequestType.Name, this.StartDate.ToString("MM/dd/yyyy"), this.EndDate.ToString("MM/dd/yyyy"));
                throw new Exception(eMsg);
            }

            this.RequestedOn = DateTime.Now;

            //Set The Request Hours
            if (this.Hours <= 0)
            {
                this.Hours = ComputeRequestHours();
            }

            var results = this.Save();
            if(results.Success)
            {
                var emlWorker = new EmailWorker();
                if (this.RequestType.Name == "Sick")
                {
                    emlWorker.SendSickDayNotification(this);

                    results.Message = string.Format("Your request for {0}, {1} has been submitted successfully.",
                        this.Agent.LastName, this.Agent.FirstName);                    
                }else
                {
                    emlWorker.SendNewRequestEmail(this);
                    results.Message = string.Format("Your request for {0} Starting on {1} Ending on {2} has been submitted for approval successfully",
                        this.RequestType.Name, this.StartDate.ToString("MM/dd/yyyy"), this.EndDate.ToString("MM/dd/yyyy"));  
                }
            }

            return results;
        }

        /// <summary>
        /// Submits Request as a sick Day
        /// </summary>
        public DataObjectOperationResult SubmitSickDay()
        {
            var reqType = this.Team.CalendarRequestTypes.FirstOrDefault(x => x.Name == "Sick");
            this.RequestType = reqType;

            //Sick Days are automaticaly approved
            this.Approved = true;
            var result = this.Save();
            if(result.Success == true)
            {
                result.Message = string.Format("{0} Has Been Reported as Sick.", this.Agent.FullName);
            }

            return result;
        }

        /// <summary>
        /// Approve or Deny An Existing Request
        /// </summary>
        public DataObjectOperationResult ApproveOrDenyRequest(bool approved, AccountUser manager, string resposeNotes)
        {
            this.Approved = approved;
            this.ResponseOn = DateTime.Now;
            this.ResponseBy = manager;
            this.ResponseNotes = resposeNotes;

            var results = this.Save();
            if (results.Success)
            {
                var emlWorker = new EmailWorker();
                emlWorker.SendRequestResponse(this);

                results.Message = string.Format("The request for {0} Starting on {1} Ending on {2} has been {3}",
                                                this.Agent.FullName, this.StartDate.ToString("MM/dd/yyyy"),this.EndDate.ToString("MM/dd/yyyy"), (this.Approved ?? false) ? "Approved" : "Denied");
            }

            return results;
        }
    }
}
