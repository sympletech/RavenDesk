using System.Collections.Generic;
using System.Linq;

namespace RavenDesk.Core.Data
{
    public interface  IDataObject
    {
        IDataContext Db { get; set; }
        string Id { get; set; }


        void AddRelationship(IDataObject relatedDataObject);
        void RemoveRelationship(IDataObject unRelatedDataObject);
        List<IDataObject> RelatedObjects { get; }

        DataObjectOperationResult Save();
    }
}