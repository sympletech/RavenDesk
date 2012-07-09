using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;

namespace TeamMgmtCal.Core.Data
{
    public interface IDataContext : IDisposable
    {
        IDocumentSession Session { get; }

        T Attach<T>(T dataObj) where T : IDataObject;

        IEnumerable<T> Attach<T>(IEnumerable<T> dataObjCollection) where T : IDataObject;
    }
}
