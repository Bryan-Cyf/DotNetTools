using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tools.Mongo
{
    /// <summary>
    /// 序列化值类型value class
    /// 用法BsonSerializer.RegisterSerializer(typeof((bool, List &lt;string>)),new newSerializer &lt;(bool, List&lt;string&gt;)&gt;());
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueTupleSerializer<T> : IBsonSerializer
    {
        public Type ValueType => typeof(T);

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            var nominalType = args.NominalType;
            var fields = nominalType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            var props = nominalType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanWrite);

            var bsonWriter = context.Writer;
            bsonWriter.WriteStartDocument();
            foreach (var field in fields)
            {
                bsonWriter.WriteName(field.Name);
                BsonSerializer.Serialize(bsonWriter, field.FieldType, field.GetValue(value));
            }
            foreach (var prop in props)
            {
                bsonWriter.WriteName(prop.Name);
                BsonSerializer.Serialize(bsonWriter, prop.PropertyType, prop.GetValue(value, null));
            }
            bsonWriter.WriteEndDocument();
        }

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var actualType = args.NominalType;
            var obj = Activator.CreateInstance(actualType);
            var bsonReader = context.Reader;

            bsonReader.ReadStartDocument();
            while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
            {
                var name = bsonReader.ReadName();

                var field = actualType.GetField(name);
                if (field != null)
                {
                    var value = BsonSerializer.Deserialize(bsonReader, field.FieldType);
                    field.SetValue(obj, value);
                }

                var prop = actualType.GetProperty(name);
                if (prop != null)
                {
                    var value = BsonSerializer.Deserialize(bsonReader, prop.PropertyType);
                    prop.SetValue(obj, value, null);
                }
            }
            bsonReader.ReadEndDocument();
            return obj;
        }
    }
}
