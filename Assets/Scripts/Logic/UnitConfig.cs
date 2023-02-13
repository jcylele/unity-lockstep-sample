using System.Collections.Generic;

namespace Logic
{
    public class UnitConfig
    {
        /// <summary>
        /// 移动速度(移动距离/s)
        /// </summary>
        public int moveSpeed;

        /// <summary>
        /// 碰撞半径
        /// </summary>
        public int radius;

        /// <summary>
        /// 预制体路径
        /// </summary>
        public string prefabPath;

        public UnitConfig(int moveSpeed, int radius, string prefabPath)
        {
            this.moveSpeed = moveSpeed;
            this.radius = radius;
            this.prefabPath = prefabPath;
        }

        private static readonly Dictionary<int, UnitConfig> UnitConfigMap = new Dictionary<int, UnitConfig>
        {
            {0, new UnitConfig(100, 25, "Prefabs/Player")},
            {1, new UnitConfig(66, 25, "Prefabs/Enemy")},
        };

        public static UnitConfig GetConfig(int sid)
        {
            UnitConfigMap.TryGetValue(sid, out var config);
            return config;
        }
    }
}