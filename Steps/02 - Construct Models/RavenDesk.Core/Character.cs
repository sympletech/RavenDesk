using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavenDesk.Core
{
    public class Character
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CatchPhrase { get; set; }

        public List<Author> Authors { get; set; }
        public List<StoryLine> StoryLines { get; set; }
        public List<Book> Books { get; set; } 
    }
}
