using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Raven.Imports.Newtonsoft.Json;
using RavenDesk.Core.Data.Attributes;

namespace RavenDesk.Core.Data
{
    public class DataObject<T> : IDataObject where T : IDataObject 
    {
        //-- Constructors

        [JsonIgnore]
        public IDataContext Db { get; set; }
        public DataObject(IDataContext db)
        {
            this.Db = db;
        }


        //-- Properties

        public string Id { get; set; }

        //-- Relationships

        /// <summary>
        /// LookUp an Existing Relationship
        /// </summary>
        private DataObjectRelationship FindRelationship(IDataObject relatedDataObject)
        {
            return (Db.Session.Query<DataObjectRelationship>()
                //Get Relationships With This in them
                .Where(x => x.DataObjects.Any(y => y.DataObjectId == this.Id)).ToList()

                //Intersect The Results with Relationships that have the Related Item in them
                .Intersect(
                    Db.Session.Query<DataObjectRelationship>()
                        .Where(x => x.DataObjects.Any(y => y.DataObjectId == relatedDataObject.Id))

                //Take The First Item in the Collection
                )).FirstOrDefault();
        }

        /// <summary>
        /// Add A relationship to object
        /// </summary>
        public void AddRelationship(IDataObject relatedDataObject)
        {
            if(string.IsNullOrEmpty(this.Id))
            {
                this.Save();
            }

            if(FindRelationship(relatedDataObject) == null)
            {
                var objRelationship = new DataObjectRelationship(this, relatedDataObject);
                Db.Session.Store(objRelationship); 
                Db.Session.SaveChanges();
            }
        }

        /// <summary>
        /// Remove an existing relationship from object
        /// </summary>
        public void RemoveRelationship(IDataObject unRelatedDataObject)
        {
            var relationship = FindRelationship(unRelatedDataObject);
            if(relationship != null)
            {
                Db.Session.Delete(relationship);
                Db.Session.SaveChanges();
            }
        }
        
        [JsonIgnore]
        public List<IDataObject> RelatedObjects
        {
            get
            {
                //Get The Relationship Entries
                var relationships = Db.Session.Query<DataObjectRelationship>()
                    .Where(x => x.DataObjects
                        .Any(y => y.DataObjectId == this.Id));

                //Read Through each relationship entry and add to results 
                //(unless it's this object or already exists in the collection)
                var results = new List<IDataObject>();
                foreach (var relationship in relationships)
                {
                    foreach (var relationshipEntry in relationship.DataObjects)
                    {
                        if ((results.Any(x =>x.Id == relationshipEntry.DataObjectId) != true)
                            && this.Id != relationshipEntry.DataObjectId)
                        {
                            results.Add(Db.Session.Load<dynamic>(relationshipEntry.DataObjectId));
                        }                        
                    }
                }

                return results;
            }
        }

        public List<DOType> QueryRelatedObjects<DOType>() where DOType : IDataObject
        {
            //Get The Relationship Entries
            var relationships = Db.Session.Query<DataObjectRelationship>()
                .Where(x => x.DataObjects
                                .Any(y =>
                                     y.DataObjectId == this.Id
                                ));

            //Read Through each relationship entry and add to results 
            //(unless it's this object or already exists in the collection)
            var results = new List<DOType>();
            foreach (var relationship in relationships)
            {
                foreach (var relationshipEntry in relationship.DataObjects)
                {
                    if ((results.Any(x => x.Id == relationshipEntry.DataObjectId) != true)
                        && this.Id != relationshipEntry.DataObjectId
                        && relationshipEntry.DataObjectType == typeof (DOType))
                    {
                        results.Add(Db.Session.Load<DOType>(relationshipEntry.DataObjectId));
                    }
                }
            }

            return results;
        }

        //-- Lookups

        /// <summary>
        /// Get A Single Object By ID
        /// </summary>
        public static T Get(IDataContext db, string id)
        {
            var result = db.Session.Query<T>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .FirstOrDefault(x => x.Id == id);

            if (result != null)
            {
                db.Attach(result);
            }

            return result;
        }

        /// <summary>
        /// Get A Single Object By Query
        /// </summary>
        public static T Get(IDataContext db, Expression<Func<T, bool>> predicate)
        {
            var result = db.Session.Query<T>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .FirstOrDefault(predicate);

            if (result != null)
            {
                db.Attach(result);
            }

            return result;
        }

        /// <summary>
        /// Returns Entire Collection of Objects
        /// </summary>
        public static IQueryable<T> GetAll(IDataContext db)
        {
            return db.Session.Query<T>();
        }

        /// <summary>
        /// Performs a Query agianst the collection
        /// </summary>
        public static IQueryable<T> Query(IDataContext db, Expression<Func<T, bool>> predicate)
        {
            return db.Session.Query<T>().Where(predicate);
        }
        
        //-- CRUD

        public DataObjectOperationResult Save()
        {
            var result = new DataObjectOperationResult();
            try
            {
                Validator.ValidateDataObject(this, ref result);
                if(result.Success == true)
                {
                    Db.Session.Store(this);
                    Db.Session.SaveChanges();

                    result.Message = "Database Update Completed Successfully";                    
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            
            return result;
        }

        public void Delete(IDataContext db)
        {
            db.Session.Delete(this);
            db.Session.SaveChanges();
            Id = null;
        }
    }

}
