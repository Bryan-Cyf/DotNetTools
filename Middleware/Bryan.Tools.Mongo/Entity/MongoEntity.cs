using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Tools.Mongo
{
    [BsonIgnoreExtraElements(Inherited = true)]
    public class MongoEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonIgnoreIfNull]
        public DateTime? CreateOn { get; set; }
        /// <summary>
        /// 创建的用户
        /// </summary>
        [BsonIgnoreIfNull]
        public string CreateBy { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonIgnoreIfNull]
        public DateTime? UpdateOn { get; set; }
        /// <summary>
        /// 更新的用户
        /// </summary>
        [BsonIgnoreIfNull]
        public string UpdateBy { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        [BsonIgnoreIfNull]
        public bool? isDel { get; set; }
        [JsonIgnore]
        [BsonIgnore]
        public ObjectId ObjectId => string.IsNullOrEmpty(Id) ? ObjectId.Empty : ObjectId.Parse(Id);
    }
}
