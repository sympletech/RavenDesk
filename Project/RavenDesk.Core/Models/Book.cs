using System;
using RavenDesk.Core.Data;
using RavenDesk.Core.Data.Attributes;
using RavenDesk.Core.Models.Interfaces;

namespace RavenDesk.Core.Models
{
    public class Book : DataObject<Book>, IBook
    {
        public Book(IDataContext db):base(db){}

        [RequiredValue(ErrorMessage = "Title Is Required")]
        public string Title { get; set; }

        public string Summary { get; set; }
        public DateTime PublishDate { get; set; }
        public bool BestSeller { get; set; }

    }
}
