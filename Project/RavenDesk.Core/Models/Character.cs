using RavenDesk.Core.Data;
using RavenDesk.Core.Data.Attributes;
using RavenDesk.Core.Models.Interfaces;

namespace RavenDesk.Core.Models
{
    public class Character : DataObject<Character>, ICharacter
    {
        public Character(IDataContext db):base(db){}

        [RequiredValue(ErrorMessage = "Name Is Required")]
        public string Name { get; set; }

        public string Description { get; set; }
        public string CatchPhrase { get; set; }
        public string Sex { get; set; }

    }
}
