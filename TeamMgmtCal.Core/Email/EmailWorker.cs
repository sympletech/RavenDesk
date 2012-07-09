using System.Configuration;
using System.Globalization;
using System.Text;
using TeamMgmtCal.Core.Data.Models;
using TeamMgmtCal.Core.Email.Templates;
using TeamMgmtCal.Core.Helpers;

namespace TeamMgmtCal.Core.Email
{
    public class EmailWorker
    {
        private IEmailer _eml;
        public IEmailer Eml
        {
            get { return _eml ?? (_eml = new Emailer()); }
            set { _eml = value; }
        }

        public void SendNewUserInfoEmail(AccountUser updatedInfo, string plainTextPassword)
        {
            var sbBody = new StringBuilder(EmailTemplates.NewUser);
            sbBody.Replace("#URL#", ConfigurationManager.AppSettings["SiteURL"]);
            sbBody.Replace("#USERNAME#", updatedInfo.Username);
            sbBody.Replace("#PASSWORD#", plainTextPassword);

            Eml.To.Add(updatedInfo.Email);
            Eml.FromAdmin();
            Eml.Subject = "Team Calendar Account Information";
            Eml.Body = sbBody.ToString();
            Eml.SendMessage();
        }

        public void SendForgetPasswordEmail(AccountUser updatedInfo, string plainTextPassword)
        {
            var sbBody = new StringBuilder(EmailTemplates.ForgotPasswordTemplate);
            sbBody.Replace("#URL#", ConfigurationManager.AppSettings["SiteURL"]);
            sbBody.Replace("#PASSWORD#", plainTextPassword);

            Eml.To.Add(updatedInfo.Email);
            Eml.FromAdmin();
            Eml.Subject = "Team Calendar Account Password Information";
            Eml.Body = sbBody.ToString();
            Eml.SendMessage();
        }

    
        public void SendNewRequestEmail(CalendarEntry requestDetail)
        {
            var sbBody = new StringBuilder(EmailTemplates.NewRequestTemplate);
            sbBody.Replace("#REQBY#", string.Format("{0}, {1}", requestDetail.Agent.LastName, requestDetail.Agent.FirstName));
            sbBody.Replace("#REQTYPE#", requestDetail.RequestType.Name);
            sbBody.Replace("#STARTDATE#", requestDetail.StartDate.ToString("MM/dd/yyyy"));
            sbBody.Replace("#ENDDATE#", requestDetail.EndDate.ToString("MM/dd/yyyy"));
            sbBody.Replace("#HOURS#", requestDetail.Hours.ToString());
            sbBody.Replace("#REQNOTES#", requestDetail.RequestNotes);

            Eml.To.Add(requestDetail.Team.Manager.Email);
            Eml.To.Add(requestDetail.Team.AssistantManager.Email);
            Eml.CC.Add(requestDetail.Agent.Email);
            Eml.FromAdmin();
            Eml.Subject = "A New Time-Off Request Has Been Submitted";
            Eml.Body = sbBody.ToString();
            Eml.SendMessage();
        }

        public void SendRequestResponse(CalendarEntry requestDetail)
        {
            string response = requestDetail.Approved.Value ? "Approved" : "Denied";

            var sbBody = new StringBuilder(EmailTemplates.RequestResponseTemplate);
            sbBody.Replace("#RESPONSE#", response);
            sbBody.Replace("#REQBY#", string.Format("{0}, {1}", requestDetail.Agent.LastName, requestDetail.Agent.FirstName));
            sbBody.Replace("#REQTYPE#", requestDetail.RequestType.Name);
            sbBody.Replace("#STARTDATE#", requestDetail.StartDate.ToString("MM/dd/yyyy"));
            sbBody.Replace("#ENDDATE#", requestDetail.EndDate.ToString("MM/dd/yyyy"));
            sbBody.Replace("#HOURS#", requestDetail.Hours.ToString());
            sbBody.Replace("#RESPONSENOTES#", requestDetail.ResponseNotes);

            Eml.To.Add(requestDetail.Agent.Email);
            Eml.CC.Add(requestDetail.Team.Manager.Email);
            Eml.CC.Add(requestDetail.Team.AssistantManager.Email);
            Eml.CC.Add(requestDetail.Team.HrAdmin.Email);

            Eml.FromAdmin();
            Eml.Subject = "A Pending Time-Off Request Has Been " + response;
            Eml.Body = sbBody.ToString();
            Eml.SendMessage();            
        }
    
        public void SendSickDayNotification(CalendarEntry requestDetail)
        {
            string agentName = string.Format("{0}, {1}", requestDetail.Agent.LastName, requestDetail.Agent.FirstName);

            var sbBody = new StringBuilder(EmailTemplates.SickNotificationTemplate);
            sbBody.Replace("#AGENTNAME#", agentName);
            sbBody.Replace("#STARTDATE#", requestDetail.StartDate.ToString("MM/dd/yyyy"));
            sbBody.Replace("#ENDDATE#", requestDetail.EndDate.ToString("MM/dd/yyyy"));
            sbBody.Replace("#HOURS#", requestDetail.Hours.ToString());
            sbBody.Replace("#REPORTEDBY#", string.Format("{0}, {1}", requestDetail.RequestedBy.LastName, requestDetail.RequestedBy.FirstName));
            sbBody.Replace("#REQNOTES#", requestDetail.RequestNotes);

            Eml.To.Add(requestDetail.Team.EmailDistro);
            Eml.To.Add(requestDetail.Agent.Email);
            Eml.To.Add(requestDetail.Team.Manager.Email);
            Eml.To.Add(requestDetail.Team.AssistantManager.Email);
            Eml.To.Add(requestDetail.Team.HrAdmin.Email);
            Eml.FromAdmin();

            Eml.Subject = agentName + " Has Called in Sick";
            Eml.Body = sbBody.ToString();
            Eml.SendMessage();            
        }
    }
}
