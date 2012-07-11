using System.Collections.Generic;
using RavenDesk.Core.Data;
using RavenDesk.Core.Data.Attributres;

namespace RavenDesk.Core.Models
{
    public class Book : DataObject<Book>
    {
        public Book(IDataContext db):base(db){}

        [RequiredValue(ErrorMessage = "Title Is Required")]
        public string Title { get; set; }

        public string Summary { get; set; }
    }
}
