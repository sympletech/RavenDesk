using System.Net.Mail;
using TeamMgmtCal.Core.Data;
using TeamMgmtCal.Core.Data.Models;

namespace TeamMgmtCal.Core.Helpers
{
    public interface IEmailer
    {
        /// <summary>
        /// Sets the to settings to all members in a given Team
        /// </summary>     
        void ToTeam(int teamID, bool includeHRAdmin);

        /// <summary>
        /// Sets the to settings to all members in a given Team
        /// </summary>     
        void ToTeam(TeamDetail team, bool includeHRAdmin);

        void ToUser(AccountUser user);
        void ToUserID(int? userID);
        void ToCSV(string csvList);

        /// <summary>
        /// Sets From Settings to the NetopsAdmin Account
        /// </summary>
        void FromAdmin();

        /// <summary>
        /// Sets From Settings to a Specific Team
        /// </summary>
        void FromTeam(TeamDetail team);

        /// <summary>
        /// Sets From Settings to the Specified User
        /// </summary>
        void FromUser(AccountUser usr);

        /// <summary>
        /// Generate The E-Mail Body using the Appropriate Template
        /// </summary>
        void GenerateBody();

        string Signature { get; set; }
        MailAddress From { get; set; }
        MailAddressCollection To { get; }
        MailAddressCollection Bcc { get; }
        MailAddressCollection CC { get; }
        string Subject { get; set; }
        string Body { get; set; }

        /// <summary>
        /// Generates a Signature From A User Account Entry
        /// </summary>
        /// <param name="usr"></param>
        void GenerateSignature(AccountUser usr);

        /// <summary>
        /// Generate The Signature of the e-mail message
        /// </summary>
        void GenerateSignature(string name);

        /// <summary>
        /// Generate The Signature of the e-mail message
        /// </summary>
        void GenerateSignature(string name, string position, string team);

        /// <summary>
        /// Send The Message On It's Way
        /// </summary>
        void SendMessage();
    }
}