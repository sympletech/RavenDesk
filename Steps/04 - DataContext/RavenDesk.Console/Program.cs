using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavenDesk.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //var crud = new Step3InitialCrud();
            //crud.CreateSomeThings();
            //crud.UpdateSomeStuff();
            //crud.NoCascadingUpdates();
            //crud.DeleteSomething();

            var dContext = new Step4_DataContext();
            dContext.InitDatabase();
            dContext.CreateSomeThings();
            //dContext.CreateSomeRelationships();

            
        }
    }
}
