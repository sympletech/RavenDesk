using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavenDesk.Core
{
    public class Book
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }

        public List<Author> Authors { get; set; }
        public List<StoryLine> StoryLines { get; set; }
        public List<Character> Characters { get; set; } 
    }
}
