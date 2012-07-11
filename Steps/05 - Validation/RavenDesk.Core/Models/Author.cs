using System.Collections.Generic;
using RavenDesk.Core.Data;
using RavenDesk.Core.Data.Attributres;

namespace RavenDesk.Core.Models
{
    public class Author : DataObject<Author>
    {
        public Author(IDataContext db):base(db){}

        [RequiredValue(ErrorMessage = "Last Name Is Required")]
        public string LastName { get; set; }
        
        [RequiredValue(ErrorMessage = "First Name Is Required")]
        public string FirstName { get; set; }

        [Email(AllowNull = false)]
        public string EmailAddress { get; set; }
    }
}
