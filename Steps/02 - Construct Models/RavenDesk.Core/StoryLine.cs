using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavenDesk.Core
{
    public class StoryLine
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public List<Book> Books { get; set; }
        public List<Author> Authors { get; set; } 
        public List<Character> Characters { get; set; } 

    }
}
