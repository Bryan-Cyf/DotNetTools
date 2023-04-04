using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Tools.Mongo
{
    public class MongoOptions
    {
        public const string SectionName = "MongoDB";

        /// <summary>
        /// 数据库连接串
        /// </summary>
        [Required]
        public string Connection { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        [Required]
        public string DatabaseName { get; set; }

        /// <summary>
        /// 是否自动创建索引
        /// </summary>
        public bool IsAutoCreateIndex { get; set; } = false;

        /// <summary>
        /// BsonChunkPool(16, 64 * 1024)
        /// </summary>
        public BsonChunkPool BsonChunkPool { get; set; }
    }
}
