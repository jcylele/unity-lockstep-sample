namespace Logic
{
    public enum GameState
    {
        None,
        Loading,
        Ready,
        Running,
        Finished
    }

    /// <summary>
    /// 常量定义
    /// </summary>
    public static class Const
    {
        /// <summary>
        /// 每逻辑帧时长(s)
        /// </summary>
        public static readonly float FrameInterval = 0.033f;

        /// <summary>
        /// 地图宽度一半
        /// </summary>
        public static readonly int MaxX = 640;

        /// <summary>
        /// 地图高度一半
        /// </summary>
        public static readonly int MaxY = 360;

        /// <summary>
        /// 玩家身边禁止刷敌范围
        /// </summary>
        public static readonly int MinSpawnDistance = 100;
    }
}