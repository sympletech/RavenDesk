using AutoMapper;
using System.Reflection;

namespace TeamMgmtCal.Core.Data.Mapping
{
    public class MappedModel
    {
        public MappedModel()
        {
            MapDefinition();
        }

        public virtual void MapDefinition()
        {
            var bindings = this.GetType().GetCustomAttributes<MapToAttribute>();
            foreach (var addMapAttribute in bindings)
            {
                var existingMap = Mapper.FindTypeMapFor(addMapAttribute.SourceType, addMapAttribute.DestinationType);
                if (existingMap == null)
                {
                    Mapper.CreateMap(addMapAttribute.SourceType, addMapAttribute.DestinationType);
                    Mapper.CreateMap(addMapAttribute.DestinationType, addMapAttribute.SourceType);
                }
            }            
        }
    }
}