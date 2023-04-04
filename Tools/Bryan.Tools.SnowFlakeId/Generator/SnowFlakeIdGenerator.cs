namespace Tools.SnowFlakeId
{
    /// <summary>
    /// 雪花算法ID 生成器
    /// </summary>
    public class SnowFlakeIdGenerator : ILongGenerator
    {
        public SnowFlakeIdGenerator()
        {
            var workerId = SnowFakeOptionsConst.WorkId;
            _id = new SnowflakeId(workerId);
        }

        /// <summary>
        /// 雪花算法ID
        /// </summary>
        private readonly SnowflakeId _id;

        /// <summary>
        /// 获取<see cref="SnowFlakeIdGenerator"/>类型的实例
        /// </summary>
        public static SnowFlakeIdGenerator Current { get; set; } = new SnowFlakeIdGenerator();

        /// <summary>
        /// 创建ID
        /// </summary>
        /// <returns></returns>
        public long Create()
        {
            return _id.NextId();
        }

        public static void Reload()
        {
            Current = new SnowFlakeIdGenerator();
        }
    }
}
