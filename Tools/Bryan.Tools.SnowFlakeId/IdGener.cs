using System;

namespace Tools.SnowFlakeId
{
    /// <summary>
    /// Id 生成器
    /// </summary>
    public static class IdGener
    {
        /// <summary>
        /// Long 生成器
        /// </summary>
        public static ILongGenerator LongGenerator { get; set; } = SnowFlakeIdGenerator.Current;

        /// <summary>
        ///基于时钟序列的 Long 生成器 
        /// </summary>
        public static ILongGenerator ClockLongGenerator { get; set; } = ClockSnowFlakeIdGenerator.Current;

        /// <summary>
        /// 用Guid创建标识
        /// </summary>
        /// <returns></returns>
        public static string GetGuid()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 创建 Long ID
        /// </summary>
        /// <returns></returns>
        public static long GetLong()
        {
            return LongGenerator.Create();
        }

        /// <summary>
        /// 创建时钟序列 Long ID
        /// </summary>
        /// <returns></returns>
        public static long GetClockLong()
        {
            return ClockLongGenerator.Create();
        }

    }
}
