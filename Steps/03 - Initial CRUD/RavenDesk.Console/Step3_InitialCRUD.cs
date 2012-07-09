using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Document;
using RavenDesk.Core;

namespace RavenDesk.Console
{
    public class Step3InitialCrud
    {
        private readonly IDocumentStore _documentStore;
        private IDocumentSession _session;

        public Step3InitialCrud()
        {
            this._documentStore = new DocumentStore
            {
                ConnectionStringName = "RavenDeskDB"

            }.Initialize();
            
        }

        public void CreateSomeThings()
        {
            var sKing = new Author
                            {
                                FirstName = "Steven",
                                LastName = "King",
                                Books = new List<Book>(),
                                Characters = new List<Character>()
                            };

            var shining = new Book
                            {
                                Title = "The Shining",
                                Summary = "A Hotel In Hell",
                                Characters = new List<Character>(),
                                Authors = new List<Author>()
                            };

            var jackTorrance = new Character
                            {
                                Name = "Jack Torrance",
                                Description = "A Little Off His Rocker",
                                CatchPhrase = "I'm Not Gonna Hurt Ya, I'm Just Gona Bash Your Brains In"
                            };

            this._session = this._documentStore.OpenSession();
            this._session.Store(sKing);
            this._session.Store(shining);
            this._session.Store(jackTorrance);
            this._session.SaveChanges();
            this._session.Dispose();
        }

        public void UpdateSomeStuff()
        {
            this._session = this._documentStore.OpenSession();

            var sKing = _session.Query<Author>().FirstOrDefault(x => x.LastName == "King");
            var shining = this._session.Query<Book>().FirstOrDefault(x => x.Title == "The Shining");
            var jackTorrance = this._session.Query<Character>().FirstOrDefault(x => x.Name == "Jack Torrance");

            sKing.Books.Add(shining);
            shining.Characters.Add(jackTorrance);

            this._session.SaveChanges();

            this._session.Dispose();
        }

        public void NoCascadingUpdates()
        {
            this._session = this._documentStore.OpenSession();

            var jackTorrance = this._session.Query<Character>().FirstOrDefault(x => x.Name == "Jack Torrance");
            jackTorrance.Description = "Not So Bad Once You Get To Know Him";

            this._session.SaveChanges();
            this._session.Dispose();

        }

        public void DeleteSomething()
        {
            this._session = this._documentStore.OpenSession();

            var jackTorrance = this._session.Query<Character>().FirstOrDefault(x => x.Name == "Jack Torrance");
            this._session.Delete(jackTorrance);

            this._session.SaveChanges();
            this._session.Dispose();
        }
    
    }
}
