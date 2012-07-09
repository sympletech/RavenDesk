using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavenDesk.Core
{
    public class Author
    {
        public string Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }

        public List<Book> Books { get; set; }
        public List<StoryLine> StoryLines { get; set; }
        public List<Character> Characters { get; set; } 
    }
}
