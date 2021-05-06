using System.Collections.Generic;

namespace testtest.Service.General
{
    public interface IpropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping
            <TSource, TDestination>();
        bool ValidMappingExistsFor<TSource, TDestination>(string fields);
    }
}