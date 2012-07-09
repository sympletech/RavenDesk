using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RavenDesk.Core.Data;
using RavenDesk.Core.Models;

namespace RavenDesk.Core
{
    public class StoryLine: DataObject<StoryLine>
    {
        public StoryLine(IDataContext db):base(db){}

        public string Title { get; set; }
        public string Description { get; set; }
    }
}
