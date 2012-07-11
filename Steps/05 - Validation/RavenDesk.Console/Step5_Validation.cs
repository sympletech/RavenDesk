using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RavenDesk.Core.Data;
using RavenDesk.Core.Models;

namespace RavenDesk.Console
{
    public class Step5_Validation
    {
        private DataContext Db { get; set; }

        public void RequiredFieldTest()
        {
            using(Db = new DataContext())
            {
                var Hemingway = new Author(Db)
                {
                    LastName = "Hemingway"
                };
                var results = Hemingway.Save();

                System.Console.WriteLine(results.Message);
                if(results.Success != true)
                {
                    foreach (var error in results.ErrorMessages)
                    {
                        System.Console.WriteLine("Field: {0} | Message {1}", error.Key, error.Value);
                    }
                }
            }

        }
    }
}
