using System;
using System.Collections.Generic;
using System.Web.Mvc;
using RavenDesk.Core.Models;
using RavenDesk.Core.Models.Interfaces;

namespace RavenDesk.MVC.Models.Books
{
    public class BookFormModel : IBook
    {
        public string Id { get; set; }
        public string PopupTitle { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime PublishDate { get; set; }
        public bool BestSeller { get; set; }


        public List<SelectListItem> AuthorAssoiationOptions { get; set; }
        public IEnumerable<IAuthor> Authors { get; set; }

        public List<SelectListItem> CharacterAssoiationOptions { get; set; }
        public IEnumerable<ICharacter> Characters { get; set; }



    }
}