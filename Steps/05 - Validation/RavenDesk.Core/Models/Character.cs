using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RavenDesk.Core.Data;
using RavenDesk.Core.Data.Attributres;
using RavenDesk.Core.Models;

namespace RavenDesk.Core
{
    public class Character : DataObject<Character>
    {
        public Character(IDataContext db):base(db){}

        [RequiredValue(ErrorMessage = "Name Is Required")]
        public string Name { get; set; }

        public string Description { get; set; }
        public string CatchPhrase { get; set; }
    }
}
