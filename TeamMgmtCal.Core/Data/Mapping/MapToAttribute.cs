using System;

namespace TeamMgmtCal.Core.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MapToAttribute : Attribute
    {
        public MapToAttribute(Type sourceType, Type destinationType)
        {
            this.SourceType = sourceType;
            this.DestinationType = destinationType;
        }

        public Type SourceType { get; set; }
        public Type DestinationType { get; set; }
    }
}