using System;
using System.Collections.Generic;
using System.Web.Mvc;
using RavenDesk.Core.Models.Interfaces;

namespace RavenDesk.MVC.Models.Characters
{
    public class CharacterFormModel : ICharacter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CatchPhrase { get; set; }
        public string Sex { get; set; }

        public List<SelectListItem> SexOptions
        {
            get
            {
                var sexOptions = new List<SelectListItem>();
                sexOptions.Add(new SelectListItem { Text = "Male", Value = "Male" });
                sexOptions.Add(new SelectListItem { Text = "Female", Value = "Female" });
                return sexOptions;
            }
        }

        public string PopupTitle { get; set; }

        public List<SelectListItem> AuthorAssoiationOptions { get; set; }
        public IEnumerable<IAuthor> Authors { get; set; }

        public List<SelectListItem> BookAssoiationOptions { get; set; }
        public IEnumerable<IBook> Books { get; set; }
    }
}