using RavenDesk.Core.Models.Interfaces;
using RavenDesk.MVC.Helpers;

namespace RavenDesk.MVC.Models.Characters
{
    public class CharacterListViewModel
    {
        public PageableSearchResults<ICharacter> Characters { get; set; }

        public string SearchTerm { get; set; }

        public int PageNum { get; set; }
        public int TotalPages { get; set; }
    }
}