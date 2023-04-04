using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Tools.Mongo
{
    public static class MongoExtension
    {

        public static IMongoCollection<T> GetCollection<T>(this IMongoDatabase mongoDatabase, MongoCollectionSettings settings = null)
        {
            return mongoDatabase.GetCollection<T>(typeof(T).Name, settings);
        }

        public static T Deserialize<T>(this BsonDocument document)
        {
            return BsonSerializer.Deserialize<T>(document);
        }

        public static List<T> DeserializeList<T>(this List<BsonDocument> documents)
        {
            return documents?.Select(x => x.Deserialize<T>()).ToList();
        }

        public static FilterDefinition<T> And<T>(this FilterDefinition<T> filter, Expression<Func<T, bool>> expression)
        {
            filter &= Builders<T>.Filter.Where(expression);
            return filter;
        }
        public static FilterDefinition<T> AndIf<T>(this FilterDefinition<T> filter, Func<bool> condition, Expression<Func<T, bool>> expression)
        {
            if (condition())
            {
                filter &= Builders<T>.Filter.Where(expression);
            }
            return filter;
        }

        public static FilterDefinition<T> Or<T>(this FilterDefinition<T> filter, Expression<Func<T, bool>> expression)
        {
            filter |= Builders<T>.Filter.Where(expression);
            return filter;
        }

        public static FilterDefinition<T> OrIf<T>(this FilterDefinition<T> filter, Func<bool> condition, Expression<Func<T, bool>> expression)
        {
            if (condition())
            {
                filter |= Builders<T>.Filter.Where(expression);
            }
            return filter;
        }
    }
}
