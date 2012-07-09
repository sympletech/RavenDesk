using System.Collections.Generic;
using RavenDesk.Core.Data;

namespace RavenDesk.Core.Models
{
    public class Author : DataObject<Author>
    {
        public Author(IDataContext db):base(db){}

        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
}
