using System;
using System.Collections.Generic;
using RavenDesk.Core.Data;
using RavenDesk.Core.Data.Attributes;

namespace RavenDesk.Core.Models.Interfaces
{
    public interface IAuthor
    {
        string Id { get; set; }

        [RequiredValue(ErrorMessage = "Last Name Is Required")]
        string LastName { get; set; }

        [RequiredValue(ErrorMessage = "First Name Is Required")]
        string FirstName { get; set; }

        [Email(AllowNull = false)]
        string EmailAddress { get; set; }

        string HomeTown { get; set; }
        DateTime BirthDate { get; set; }
        bool Alive { get; set; }
    }
}