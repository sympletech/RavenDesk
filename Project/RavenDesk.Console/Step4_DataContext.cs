using RavenDesk.Core;
using RavenDesk.Core.Data;
using RavenDesk.Core.Models;

namespace RavenDesk.Console
{
    public class Step4_DataContext
    {
        public DataContext Db { get; set; }
        
        public void InitDatabase()
        {
            using (Db = new DataContext())
            {
                Db.InitilizeDatabase();
            }
        }

        public void CreateSomeThings()
        {
            using (Db = new DataContext())
            {
                var sKing = new Author(Db)
                                {
                                    FirstName = "Steven",
                                    LastName = "King",
                                };

                var shining = new Book(Db)
                                  {
                                      Title = "The Shining",
                                      Summary = "A Hotel In Hell",
                                  };

                var jackTorrance = new Character(Db)
                                       {
                                           Name = "Jack Torrance",
                                           Description = "A Little Off His Rocker",
                                           CatchPhrase = "I'm Not Gonna Hurt Ya, I'm Just Gona Bash Your Brains In"
                                       };

                sKing.Save();
                shining.Save();
                jackTorrance.Save();
            }
        }

        public void CreateSomeRelationships()
        {
            using (Db = new DataContext())
            {
                var sKing = Author.Get(Db, x => x.LastName == "King");
                var shining = Book.Get(Db, x => x.Title == "The Shining");
                var jackTorrance = Character.Get(Db, x => x.Name == "Jack Torrance");

                sKing.AddRelationship(shining);
                sKing.AddRelationship(jackTorrance);
                var result = sKing.Save();

                shining.AddRelationship(jackTorrance);
                result = shining.Save();

            }
        }    
    }
}
