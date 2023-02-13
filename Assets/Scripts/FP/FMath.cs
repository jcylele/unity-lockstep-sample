namespace FixMath
{
    /// <summary>
    /// 定点数学库
    /// </summary>
    public static class FMath
    {
        public static readonly FPoint PI = new FPoint(3.14159265f);

        /// <summary>
        /// sin值查找表
        /// </summary>
        private static readonly int[] SIN_TAB =
        {
            0, 245, 491, 735, 980, 1224, 1467, 1710, 1951, 2191,
            2430, 2667, 2903, 3137, 3369, 3599, 3827, 4052, 4276,
            4496, 4714, 4929, 5141, 5350, 5556, 5758, 5957, 6152,
            6344, 6532, 6716, 6895, 7071, 7242, 7410, 7572, 7730,
            7883, 8032, 8176, 8315, 8449, 8577, 8701, 8819, 8932,
            9040, 9142, 9239, 9330, 9415, 9495, 9569, 9638, 9700,
            9757, 9808, 9853, 9892, 9925, 9952, 9973, 9988, 9997, 10000
        };

        /// <summary>
        /// SIN_TAB*系数
        /// </summary>
        private static readonly FPoint SIN_RATIO = new FPoint(0.0001f);

        /// <summary>
        /// 一圈分成多少份，份数越高精度越高
        /// </summary>
        private static readonly int TAB_N = 256;


        public static FPoint Sin(FPoint radian)
        {
            var n = (radian * TAB_N / (2 * PI)).ToInt();
            n %= TAB_N;
            var s = 0;
            if (n >= 0 && n < (TAB_N / 4))
            {
                s = SIN_TAB[n];
            }
            else if (n < (TAB_N / 2))
            {
                s = SIN_TAB[(TAB_N / 2) - n];
            }
            else if (n < (TAB_N * 3 / 4))
            {
                s = -SIN_TAB[n - (TAB_N / 2)];
            }
            else
            {
                s = -SIN_TAB[TAB_N - n];
            }

            return SIN_RATIO * s;
        }

        public static FPoint Sqrt(FPoint p1)
        {
            return FPoint.Sqrt(p1);
        }

        public static FPoint Abs(FPoint p1)
        {
            return FPoint.Abs(p1);
        }

        
    }
}