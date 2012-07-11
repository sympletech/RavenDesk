using System;
using RavenDesk.Core.Data.Attributes;

namespace RavenDesk.Core.Models.Interfaces
{
    public interface IBook
    {
        [RequiredValue(ErrorMessage = "Title Is Required")]
        string Title { get; set; }

        string Summary { get; set; }
        DateTime PublishDate { get; set; }
        bool BestSeller { get; set; }
        string Id { get; set; }
    }
}