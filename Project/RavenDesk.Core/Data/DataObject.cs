using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Raven.Imports.Newtonsoft.Json;

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

        private DataObjectRelationship FindRelationship(IDataObject relatedDataObject)
        {
            return Db.Session.Query<DataObjectRelationship>()
                .FirstOrDefault(x =>
                    x.DataObjects.Any(y => y.DataObjectId == this.Id)
                    && x.DataObjects.Any(y => y.DataObjectId == relatedDataObject.Id));            
        }

        public void AddRelationship(IDataObject relatedDataObject)
        {
            if(FindRelationship(relatedDataObject) == null)
            {
                var objRelationship = new DataObjectRelationship(this, relatedDataObject);
                Db.Session.Store(objRelationship); 
                Db.Session.SaveChanges();
            }
        }

        public void RemoveRelationship(IDataObject unRelatedDataObject)
        {
            var relationship = FindRelationship(unRelatedDataObject);
            if(relationship != null)
            {
                Db.Session.Delete(relationship);
            }
        }
        
        [JsonIgnore]
        public IQueryable<DataObjectRelationship> RelatedObjects
        {
            get
            {
                return Db.Session.Query<DataObjectRelationship>()
                    .Where(x => x.DataObjects
                        .Any(y => y.DataObjectId == this.Id));
            }
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
                Db.Session.Store(this);
                Db.Session.SaveChanges();

                result.Message = "Database Update Completed Successfully";
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
    
    
    
        //public void ProcessRelatedObjects()
        //{
        //    //Remove any possible duplicate entries
        //    this.RelatedObjects = this.RelatedObjects.Distinct().ToList();

        //    foreach (var dataObject in this.RelatedObjects)
        //    {
        //        //Strip the related objects from the collection to prevent self-refrencing serialization
        //        dataObject.RelatedObjects = null;

        //        //Make Sure Realted object refrences this in it's related objects collection
        //        IDataObject relObj = Db.Session.Load<dynamic>(dataObject.Id);
        //        relObj.RelatedObjects = relObj.RelatedObjects ?? new List<IDataObject>();
        //        if (relObj.RelatedObjects.Contains(this) != true)
        //        {
        //            relObj.RelatedObjects.Add(this);
        //            relObj.RelatedObjects.FirstOrDefault(x => x.Id == this.Id).RelatedObjects = null;
        //        }
        //    }

        //    //if (this.Id != null)
        //    //{
        //    //    //Find any Objects that list this as a related object that are not in the related object collection
        //    //    var rObjects = Db.Session.Query<IDataObject>().Where(x => x.RelatedObjects.Any(y => y.Id == this.Id));
        //    //    var nonRObjects = rObjects.Where(x => this.RelatedObjects.Any(y => y.Id == x.Id) != true);
        //    //    foreach (var dataObject in nonRObjects)
        //    //    {
        //    //        dataObject.RelatedObjects.Remove(this);
        //    //    }
        //    //}
        //}
    }



}
