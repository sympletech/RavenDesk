using System;

namespace RavenDesk.Core.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredValueAttribute : Attribute
    {
        public string ErrorMessage { get; set; }
    }
}
