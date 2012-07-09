using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TeamMgmtCal.Core.Data.Models
{
    public class EMailLogEntry : DataObject<EMailLogEntry>
    {
        //-- Constructors

        public EMailLogEntry(IDataContext db) : base(db){}


        //-- Properties

        public MailAddressCollection To { get; set; }
        public MailAddressCollection CC { get; set; }
        public MailAddressCollection BCC { get; set; }
        
        public string Subject { get; set; }
        public string Body { get; set; }

        public DateTime SentOn { get; set; }
    }
}
