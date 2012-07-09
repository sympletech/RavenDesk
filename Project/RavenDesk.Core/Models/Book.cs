using System.Collections.Generic;
using RavenDesk.Core.Data;

namespace RavenDesk.Core.Models
{
    public class Book : DataObject<Book>
    {
        public Book(IDataContext db):base(db){}

        public string Title { get; set; }
        public string Summary { get; set; }
    }
}
