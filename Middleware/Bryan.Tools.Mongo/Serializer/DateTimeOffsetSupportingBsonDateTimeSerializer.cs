using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Tools.Mongo
{
    public class DateTimeOffsetSupportingBsonDateTimeSerializer : StructSerializerBase<DateTimeOffset>,
                 IRepresentationConfigurable<DateTimeOffsetSupportingBsonDateTimeSerializer>
    {
        private BsonType _representation;
        private string StringSerializationFormat = "YYYY-MM-ddTHH:mm:ss.FFFFFFK";

        public DateTimeOffsetSupportingBsonDateTimeSerializer() : this(BsonType.DateTime)
        {
        }

        public DateTimeOffsetSupportingBsonDateTimeSerializer(BsonType representation)
        {
            switch (representation)
            {
                case BsonType.String:
                case BsonType.DateTime:
                    break;
                default:
                    throw new ArgumentException(string.Format("{0} is not a valid representation for {1}", representation, this.GetType().Name));
            }

            _representation = representation;
        }

        public BsonType Representation => _representation;

        public override DateTimeOffset Deserialize(BsonDeserializationContext context,
                                                   BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;

            BsonType bsonType = bsonReader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.String:
                    var stringValue = bsonReader.ReadString();
                    return DateTimeOffset.ParseExact
                        (stringValue, StringSerializationFormat, DateTimeFormatInfo.InvariantInfo);

                case BsonType.DateTime:
                    var dateTimeValue = bsonReader.ReadDateTime();
                    return DateTimeOffset.FromUnixTimeMilliseconds(dateTimeValue);

                default:
                    throw CreateCannotDeserializeFromBsonTypeException(bsonType);
            }
        }

        public override void Serialize
           (BsonSerializationContext context, BsonSerializationArgs args, DateTimeOffset value)
        {
            var bsonWriter = context.Writer;

            switch (_representation)
            {
                case BsonType.String:
                    bsonWriter.WriteString(value.ToString
                          (StringSerializationFormat, DateTimeFormatInfo.InvariantInfo));
                    break;

                case BsonType.DateTime:
                    bsonWriter.WriteDateTime(value.ToUnixTimeMilliseconds());
                    break;

                default:
                    var message = string.Format("'{0}' is not a valid DateTimeOffset representation.", _representation);
    
                    throw new BsonSerializationException(message);
            }
        }

        public DateTimeOffsetSupportingBsonDateTimeSerializer WithRepresentation(BsonType representation)
        {
            if (representation == _representation)
            {
                return this;
            }
            return new DateTimeOffsetSupportingBsonDateTimeSerializer(representation);
        }

        IBsonSerializer IRepresentationConfigurable.WithRepresentation(BsonType representation)
        {
            return WithRepresentation(representation);
        }

        protected new Exception CreateCannotDeserializeFromBsonTypeException(BsonType bsonType)
        {
            var message = string.Format("Cannot deserialize a '{0}' from BsonType '{1}'.",
                BsonUtils.GetFriendlyTypeName(ValueType),
                bsonType);
            return new FormatException(message);
        }
    }
}
