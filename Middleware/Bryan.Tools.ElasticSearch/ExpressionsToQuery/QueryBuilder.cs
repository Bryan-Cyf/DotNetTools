using System;
using System.Linq.Expressions;
using System.Reflection;
using Nest;

namespace Tools.Elastic
{
    /// <summary>
    /// QueryContainer建造者
    /// </summary>
    public class QueryBuilder<T> where T : ElasticEntity
    {
        private readonly MappingIndex _mappingIndex;

        public QueryBuilder()
        {
            _mappingIndex = InitMappingInfo();
        }

        public QueryContainer GetQueryContainer(Expression<Func<T, bool>> expression)
        {
            return ExpressionsGetQuery.GetQuery(expression, _mappingIndex);
        }

        public MappingIndex GetMappingIndex()
        {
            return _mappingIndex;
        }

        private static MappingIndex InitMappingInfo()
        {
            var type = typeof(T);
            var mapping = new MappingIndex
            {
                Type = type,
                IndexName = IndexHelper.GetIndexName<T>()
            };
            foreach (var property in type.GetProperties())
                mapping.Columns.Add(new MappingColumn
                {
                    PropertyInfo = property.PropertyType,
                    PropertyName = property.Name,
                    SearchName = FiledHelp.GetValues(property.PropertyType.Name, property.Name)
                });
            return mapping;
        }
    }
}