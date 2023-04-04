using System;

namespace Tools.Elastic
{
    public class ElasticEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime? UpdateTime { get; set; }
    }
}