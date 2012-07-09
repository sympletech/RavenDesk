using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using TeamMgmtCal.Core.Data.Models;

namespace TeamMgmtCal.Core.Data
{
    public class DataContext : IDataContext
    {
        private readonly IDocumentStore _documentStore;
        private IDocumentSession _session;
        public IDocumentSession Session { get { return this._session; } }
        
        public DataContext()
        { 
            this._documentStore = new DocumentStore
                                     {
                                         ConnectionStringName = "MainDB",
                                         Conventions = new DocumentConvention
                                         {
                                             MaxNumberOfRequestsPerSession = 500
                                         }
                                         
                                     }.Initialize();
            
            this._session = this._documentStore.OpenSession();
        }

        public void CreateIndex<T>(string indexName)
        {
            this.Session.Advanced.DocumentStore.DatabaseCommands.PutIndex(indexName, new IndexDefinitionBuilder<T>
            {
                Map = documents => documents.Select(entity => new { })
            });            
        }

        public T Attach<T>(T dataObj) where T : IDataObject
        {
            dataObj.Db = this;
            return dataObj;
        }

        public IEnumerable<T> Attach<T>(IEnumerable<T> dataObjCollection) where T : IDataObject
        {
            if (dataObjCollection != null)
            {
                foreach (var dataObject in dataObjCollection)
                {
                    dataObject.Db = this;
                }
            }
            return dataObjCollection;
        }
        
        public void Dispose()
        {
            _session.Dispose();
        }

    }
}
