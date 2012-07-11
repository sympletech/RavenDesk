using System;
using System.Collections.Generic;
using Raven.Client;

namespace RavenDesk.Core.Data
{
    public interface IDataContext : IDisposable
    {
        IDocumentSession Session { get; }

        void Attach(IDataObject dataObj);

        void Attach(IEnumerable<IDataObject> dataObjCollection);
    }
}