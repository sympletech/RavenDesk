using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavenDesk.Core.Data
{
    public class DataObjectRelationship
    {
        public DataObjectRelationship()
        {
        }

        public DataObjectRelationship(IDataObject dataObjectA, IDataObject dataObjectB)
        {
            this.DataObjects = new List<DataObjectRelationshipEntry>();

            this.DataObjects.Add(new DataObjectRelationshipEntry
                           {
                               DataObjectId = dataObjectA.Id,
                               DataObjectType = dataObjectA.GetType()
                           });

            this.DataObjects.Add(new DataObjectRelationshipEntry
            {
                DataObjectId = dataObjectB.Id,
                DataObjectType = dataObjectB.GetType()
            });
        }

        public List<DataObjectRelationshipEntry> DataObjects { get; set; }


    }

    public class DataObjectRelationshipEntry
    {
        public string DataObjectId { get; set; }
        public Type DataObjectType { get; set; }
    }
}
