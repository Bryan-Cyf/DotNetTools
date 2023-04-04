
using Microsoft.Extensions.Options;

namespace Tools.SnowFlakeId
{
    public class SnowFakeOptions
    {
        public const string SectionName = "SnowFlakeId";

        /// <summary>
        /// 工作节点ID，范围 0~1023
        /// </summary>
        public int WorkId { get; set; }
    }
}
