using System;
using System.Collections.Generic;
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
                var dObjects = new List<IDataObject>();
                dObjects.Add(new Author(Db)
                                {
                                    FirstName = "Steven",
                                    LastName = "King",
                                });
                dObjects.Add(new Book(Db)
                                  {
                                      Title = "The Shining",
                                      Summary = "A Hotel In Hell",
                                  });
                dObjects.Add(new Book(Db)
                {
                    Title = "IT",
                    Summary = "A Clown In The Gutter",
                });
                dObjects.Add(new Character(Db)
                    {
                        Name = "Jack Torrance",
                        Description = "A Little Off His Rocker",
                        CatchPhrase = "I'm Not Gonna Hurt Ya, I'm Just Gona Bash Your Brains In"
                    });
                dObjects.Add(new Character(Db)
                {
                    Name = "Pennywise the Dancing Clown",
                    Description = "Freaky Clown With Some Dental Problems",
                    CatchPhrase = "Want a balloon?"
                });

                dObjects.ForEach(x=>x.Save());

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
                shining.AddRelationship(jackTorrance);

                var it = Book.Get(Db, x => x.Title == "IT");
                var pennywise = Character.Get(Db, x => x.Name == "Pennywise the Dancing Clown");

                sKing.AddRelationship(it);
                sKing.AddRelationship(pennywise);
                it.AddRelationship(pennywise);


            }
        }
  
        public void VerifyRelationships()
        {
            var sKing = Author.Get(Db, x => x.LastName == "King");
            System.Console.WriteLine("Steven King Relationships");
            foreach (var rObj in sKing.RelatedObjects)
            {
                System.Console.WriteLine(rObj.Id);
            }

            var shining = Book.Get(Db, x => x.Title == "The Shining");
            System.Console.WriteLine("\n Shining Relationships");
            foreach (var rObj in shining.RelatedObjects)
            {
                System.Console.WriteLine(rObj.Id);
            }

            var it = Book.Get(Db, x => x.Title == "IT");
            System.Console.WriteLine("\n IT Relationships");
            foreach (var rObj in it.RelatedObjects)
            {
                System.Console.WriteLine(rObj.Id);
            }
        }
    }
}
