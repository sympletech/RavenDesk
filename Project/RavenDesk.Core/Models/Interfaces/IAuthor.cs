using System;
using RavenDesk.Core.Data.Attributes;

namespace RavenDesk.Core.Models.Interfaces
{
    public interface IAuthor
    {
        [RequiredValue(ErrorMessage = "Last Name Is Required")]
        string LastName { get; set; }

        [RequiredValue(ErrorMessage = "First Name Is Required")]
        string FirstName { get; set; }

        [Email(AllowNull = false)]
        string EmailAddress { get; set; }

        string HomeTown { get; set; }
        DateTime BirthDate { get; set; }
        bool Alive { get; set; }
        string Id { get; set; }
    }
}