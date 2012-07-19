using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RavenDesk.Core.Data;
using RavenDesk.Core.Models;
using RavenDesk.Core.Models.Interfaces;

namespace RavenDesk.MVC.Models.Authors
{
    public class AuthorFormModel : IAuthor
    {
        public string Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string EmailAddress { get; set; }
        public string HomeTown { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Alive { get; set; }
        


        public string PopupTitle { get; set; }
    }
}