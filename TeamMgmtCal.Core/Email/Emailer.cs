using System;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using TeamMgmtCal.Core.Data;
using TeamMgmtCal.Core.Data.Models;
using TeamMgmtCal.Core.Helpers;

namespace TeamMgmtCal.Core.Email
{
    public class Emailer : MailMessage, IEmailer
    {
        public DataContext Db { get; set; }

        protected string SMTPServer
        {
            get
            {
                string smtpServer = ConfigurationManager.AppSettings["SMTPServer"];
                if (string.IsNullOrEmpty(smtpServer))
                {
                    smtpServer = "smtpgw1.maintech1.com";
                }
                return smtpServer;
            }
        }
        protected int SMTPPort
        {
            get
            {
                int port;
                if(int.TryParse(ConfigurationManager.AppSettings["SMTPPort"], out port))
                {
                    return port;
                }
                return 25;
            }
        }
        protected string EmailHtmlTemplate
        {
            get
            {
                return Templates.EmailTemplates.Master;
            }
        }
        protected string EmailPlainTemplate
        {
            get
            {
                return Templates.EmailTemplates.Master_PlainText;
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new E-Mail Message
        /// </summary>
        public Emailer()
        {
            
            this.IsBodyHtml = true;
        }

        /// <summary>
        /// Create a new E-Mail Message and Assign The To And From Values
        /// </summary>
        public Emailer(string to, AccountUser fromUser)
        {
            this.To.Add(new MailAddress(to));
            this.From = new MailAddress(fromUser.Email, fromUser.LastName + "," + fromUser.FirstName);
            this.IsBodyHtml = true;

            GenerateSignature(fromUser);
        }

        /// <summary>
        /// Create a new E-Mail Message and Assign The To And From Values
        /// </summary>
        public Emailer(string to, string fromName, string fromEmail)
        {
            this.To.Add(new MailAddress(to));
            this.From = new MailAddress(fromEmail, fromName);
            this.IsBodyHtml = true;

            GenerateSignature(fromName);
        }

        #endregion

        #region To Methods

        /// <summary>
        /// Sets the to settings to all members in a given Team
        /// </summary>     
        public void ToTeam(int teamID, bool includeHRAdmin)
        {
            var team = Db.Session.Load<TeamDetail>(teamID);
            if (team != null)
            {
                this.ToTeam(team, includeHRAdmin);
            }
        }

        /// <summary>
        /// Sets the to settings to all members in a given Team
        /// </summary>     
        public void ToTeam(TeamDetail team, bool includeHRAdmin)
        {
            foreach (var usr in team.TeamMembers)
            {
                this.ToUser(usr);
            }

            this.ToUser(team.Manager);
            this.ToUser(team.AssistantManager);


            if (regExHelpers.IsValidEmail(team.EmailDistro, false))
            {
                this.To.Add(team.EmailDistro);
            }

            if (includeHRAdmin)
            {
                this.ToUser(team.HrAdmin);
            }
        }

        public void ToUser(AccountUser user)
        {
            if (user != null)
            {
                if (regExHelpers.IsValidEmail(user.Email, false))
                {
                    this.To.Add(user.Email);
                }
            }
        }

        public void ToUserID(int? userID)
        {
            var usr = Db.Session.Load<AccountUser>(userID);
            this.ToUser(usr);
        }

        public void ToCSV(string csvList)
        {
            if (!string.IsNullOrEmpty(csvList))
            {
                foreach (var eml in csvList.Split(','))
                {
                    if (regExHelpers.IsValidEmail(eml, false))
                    {
                        this.To.Add(eml);
                    }
                }
            }
        }

        #endregion

        #region From Methods

        /// <summary>
        /// Sets From Settings to the NetopsAdmin Account
        /// </summary>
        public void FromAdmin()
        {
            this.From = new MailAddress("netops@maintech.com", "Maintech Netops Administrator");
            GenerateSignature("Maintech Netops Administrator");
        }

        /// <summary>
        /// Sets From Settings to a Specific Team
        /// </summary>
        public void FromTeam(TeamDetail team)
        {
            this.From = new MailAddress(team.EmailDistro, team.Name);
        }

        /// <summary>
        /// Sets From Settings to the Specified User
        /// </summary>
        public void FromUser(AccountUser usr)
        {
            this.From = new MailAddress(usr.Email, usr.LastName + "," + usr.FirstName);
            GenerateSignature(usr);
            this.Bcc.Add(usr.Email);
        }

        #endregion    

        #region Body

        /// <summary>
        /// Generate The E-Mail Body using the Appropriate Template
        /// </summary>
        public void GenerateBody()
        {
            string template;

            if (string.IsNullOrEmpty(this.Signature))
                GenerateSignature(this.From.DisplayName);

            if (this.IsBodyHtml)
            {
                template = this.EmailHtmlTemplate;
                this.Signature = this.Signature.Replace("\n", "<br />");
            }
            else
                template = this.EmailPlainTemplate;



            this.Body = template.
                Replace("#Body#", this.Body).
                Replace("#Signature#", this.Signature);
        }

        #endregion

        #region Signature

        public string Signature { get; set; }

        /// <summary>
        /// Generates a Signature From A User Account Entry
        /// </summary>
        /// <param name="usr"></param>
        public void GenerateSignature(AccountUser usr)
        {
            this.GenerateSignature(usr.FirstName + " " + usr.LastName, 
                usr.Position, 
                "");
        }

        /// <summary>
        /// Generate The Signature of the e-mail message
        /// </summary>
        public void GenerateSignature(string name)
        {
            GenerateSignature(name, null, null);
        }

        /// <summary>
        /// Generate The Signature of the e-mail message
        /// </summary>
        public void GenerateSignature(string name, string position, string team)
        {
            var sbSig = new StringBuilder("--\n");
            sbSig.AppendLine(name);
            sbSig.AppendLine(position);
            sbSig.AppendLine(team);

            this.Signature = sbSig.ToString();
        }

        #endregion    
    
        #region Send Message

        /// <summary>
        /// Send The Message On It's Way
        /// </summary>
        public void SendMessage()
        {
            //Clean Out Any Duplicate E-Mail 
            var to = this.To.Select(x=>x.Address.ToLower()).Distinct().ToList();
            int tCnt = this.To.Count;
            for (int i = 0; i < tCnt; i++)
            {
                this.To.RemoveAt(0);
            }
            foreach (var t in to)
            {
                this.To.Add(t);
            }

            var cc = this.CC.Select(x => x.Address.ToLower()).Distinct().ToList();
            int ccCnt = this.CC.Count;
            for (int i = 0; i < ccCnt; i++)
            {
                this.CC.RemoveAt(0);
            }
            foreach (var c in cc)
            {
                this.CC.Add(c);
            }

            string bodyTemp = this.Body;

            try
            {
                GenerateBody();
                var smtpClient = new SmtpClient(SMTPServer, SMTPPort);
                smtpClient.Send(this);

                LogEmail();
            }
            catch
            {
            }

            //Set the body back to it's raw value without formatting 
            //to prevent problems when a message is sent multiple times
            this.Body = bodyTemp;
        }

        private void LogEmail()
        {
            var eLogEntry = new EMailLogEntry(Db)
                {
                    To = this.To,
                    CC = this.CC,
                    BCC = this.Bcc,
                    Subject = this.Subject,
                    Body = this.Body,
                    SentOn = DateTime.Now
                };
            eLogEntry.Save();
        }

        #endregion    
    }
}
