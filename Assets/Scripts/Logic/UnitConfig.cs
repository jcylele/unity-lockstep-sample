using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// temporary config for units
    /// </summary>
    public class UnitConfig
    {
        /// <summary>
        /// move speed, unit: logical distance unit/second
        /// </summary>
        public int moveSpeed;

        /// <summary>
        /// collision radius, unit: logical distance unit
        /// </summary>
        public int radius;

        /// <summary>
        /// prefab path
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