using System;
using System.Linq;
using System.Reflection;

namespace RavenDesk.Core.Data.Attributes
{
    public class Validator
    {
        public static void ValidateDataObject(IDataObject dataObj, ref DataObjectOperationResult result)
        {
            CheckRequiredProperties(dataObj, ref result);
            CheckEMailProperties(dataObj, ref result);
        }

        private static void CheckRequiredProperties(IDataObject dataObj, ref DataObjectOperationResult result)
        {
            //Get Required Properties
            var requiredProperties = dataObj.GetType().GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(RequiredValueAttribute)));

            //Check to Ensure Required Properties are not Null or empty
            foreach (var reqProp in requiredProperties)
            {
                var prop = dataObj.GetType().GetProperty(reqProp.Name);
                var propValue = prop.GetValue(dataObj);
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
                    result.Message = "A Required Value is Missing";
                    result.ErrorMessages.Add(reqProp.Name, reqAttrib.ErrorMessage);
                    result.Success = false;
                }
            }
        }

        private static void CheckEMailProperties(IDataObject dataObj, ref DataObjectOperationResult result)
        {
            //Get E-Mail Properties
            var emailProperties = dataObj.GetType().GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(EmailAttribute)));

            //Check to Ensure Required Properties are Valid E-Mail Addresses
            foreach (var emlProp in emailProperties)
            {
                var prop = dataObj.GetType().GetProperty(emlProp.Name);
                var propValue = prop.GetValue(dataObj);
                var emlAttrib = prop.GetCustomAttribute<EmailAttribute>();
                if (emlAttrib.IsValid(propValue) != true)
                {
                    result.Message = "An Error Occured While Validating Input";
                    result.ErrorMessages.Add(emlProp.Name, "Not A Valid E-Mail Address");
                    result.Success = false;
                }
            }

        }    
    }
}
