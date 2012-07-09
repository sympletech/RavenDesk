using System;

namespace TeamMgmtCal.Core.Data.Attributres
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredValueAttribute : Attribute
    {
        public string ErrorMessage { get; set; }
    }
}
