using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using Raven.Imports.Newtonsoft.Json;
using TeamMgmtCal.Core.Data.Attributres;
using TeamMgmtCal.Core.Data.Models;

namespace TeamMgmtCal.Core.Data
{
    public interface  IDataObject
    {
        IDataContext Db { get; set; }
        string Id { get; set; }
    }

    public class DataObject<T> : IDataObject where T : IDataObject 
    {
        //-- Constructors
        [JsonIgnore]
        public IDataContext Db { get; set; }
        public DataObject(IDataContext db)
        {
            this.Db = db;
        }

        public static T Get(IDataContext db, string id)
        {
            var result = db.Session.Query<T>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .FirstOrDefault(x => x.Id == id);

            if(result != null)
            {
                db.Attach(result);
            }

            return result;
        }

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

        public static cTo UpCastTo<cTo>(T baseObject)
        {
            Mapper.CreateMap<T, cTo>();
            return Mapper.Map<cTo>(baseObject);
        }

        //-- Properties
        public string Id { get; set; }
        
        
        //-- Lookups

        public static IQueryable<T> GetAll(IDataContext db)
        {
            return db.Session.Query<T>();
        }

        public static IQueryable<T> Query(IDataContext db, Expression<Func<T, bool>> predicate)
        {
            return db.Session.Query<T>().Where(predicate);
        }

        public static List<T> AttachCollectionToDatabase(IDataContext db, IEnumerable<T> collection)
        {
            var results = new List<T>();
            foreach (var obj in collection)
            {
                obj.Db = db;
            }
            return results;
        }

        //-- CRUD
        public DataObjectOperationResult Save()
        {
            var result = new DataObjectOperationResult();

            CheckRequiredProperties(ref result);
            CheckEMailProperties(ref result);
            UpdateLinkedProperties(ref result);

            if (result.Success)
            {
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
            }
            
            return result;
        }

        #region Validation

        private const string PropertyValidationErrorMessage = "There Are Some Required Values That Are Missing or Incorrect";

        private void CheckRequiredProperties(ref DataObjectOperationResult result)
        {
            //Get Required Properties
            var requiredProperties = this.GetType().GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(RequiredValueAttribute)));

            //Check to Ensure Required Properties are not Null or empty
            foreach (var reqProp in requiredProperties)
            {
                var prop = this.GetType().GetProperty(reqProp.Name);
                var propValue = prop.GetValue(this);
                bool isNull = false;

                switch (prop.PropertyType.Name.ToLower())
                {
                    case "string":
                        isNull = string.IsNullOrEmpty((string)propValue);
                        break;
                    default:
                        isNull = propValue == null;
                        break;
                }

                if (isNull)
                {
                    var reqAttrib = prop.GetCustomAttribute<RequiredValueAttribute>();
                    result.Message = PropertyValidationErrorMessage;
                    result.ErrorMessages.Add(reqProp.Name, reqAttrib.ErrorMessage);
                    result.Success = false;
                }
            }
        }

        private void CheckEMailProperties(ref DataObjectOperationResult result)
        {
            //Get E-Mail Properties
            var emailProperties = this.GetType().GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(EmailAttribute)));

            //Check to Ensure Required Properties are Valid E-Mail Addresses
            foreach (var emlProp in emailProperties)
            {
                var prop = this.GetType().GetProperty(emlProp.Name);
                var prop_value = prop.GetValue(this);
                var emlAttrib = prop.GetCustomAttribute<EmailAttribute>();
                if(emlAttrib.IsValid(prop_value) != true)
                {
                    result.Message = PropertyValidationErrorMessage;
                    result.ErrorMessages.Add(emlProp.Name, "Not A Valid E-Mail Address");
                    result.Success = false;
                }
            }

        }

        #endregion

        #region UpdateOnSave

        public void UpdateLinkedProperties(ref DataObjectOperationResult result)
        {
            try
            {
                //Get Properties To Update
                var updateProperties = this.GetType().GetProperties()
                    .Where(x => Attribute.IsDefined(x, typeof(UpdateOnSaveAttribute)));

                foreach (var uProp in updateProperties)
                {
                    var prop = this.GetType().GetProperty(uProp.Name);
                    var propValue = uProp.GetValue(this);

                    if(propValue != null)
                    {
                        var id = propValue.GetType().GetProperty("Id").GetValue(propValue).ToString();
                        var updatedObj = Db.Session.Load<dynamic>(id);
                        prop.SetValue(this, updatedObj);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Unable To Update Linked Properties";
            }

        }

        #endregion

        public void Delete(IDataContext db)
        {
            db.Session.Delete(this);
            db.Session.SaveChanges();
            this.Id = null;
        }
    }



}
