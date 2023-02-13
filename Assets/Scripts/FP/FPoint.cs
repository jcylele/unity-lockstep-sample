using System;
using Log;
using ProtoBuf;

namespace FixMath
{
    // <see cref="https://www.cnblogs.com/cnxkey/articles/9721595.html"/>
    /// <summary>
    /// 定点数，高位表示整数，低位表示小数
    /// </summary>
    [ProtoContract]
    public struct FPoint
    {
        /// <summary>
        /// 小数部分占用的位数，
        /// 位数越多精度越高同时数值范围变小
        /// </summary>
        public const int FRAC_BIT = 16;

        public static readonly FPoint Zero = new FPoint(0);
        public static readonly FPoint One = new FPoint(1);

        [ProtoMember(1)] public long Val { get; set; }

        #region 构造函数

        public FPoint(int x)
        {
            Val = x << FRAC_BIT;
        }

        public FPoint(float x)
        {
            Val = (long) (x * (1 << FRAC_BIT));
        }

        public FPoint(long x)
        {
            Val = x << FRAC_BIT;
        }

        private FPoint(long val, bool bReal)
        {
            if (bReal)
            {
                Val = val;
            }
            else
            {
                Val = val << FRAC_BIT;
            }
        }

        #endregion

        #region +

        public static FPoint operator +(FPoint p1, FPoint p2)
        {
            return new FPoint(p1.Val + p2.Val, true);
        }

        public static FPoint operator +(FPoint p1, int a)
        {
            return new FPoint(p1.Val + (a << FRAC_BIT), true);
        }

        #endregion

        #region -

        public static FPoint operator -(FPoint p1, FPoint p2)
        {
            return new FPoint(p1.Val - p2.Val, true);
        }

        public static FPoint operator -(FPoint p1)
        {
            return new FPoint(-p1.Val, true);
        }

        public static FPoint operator -(FPoint p1, int a)
        {
            return new FPoint(p1.Val - (a << FRAC_BIT));
        }

        #endregion

        #region *

        public static FPoint operator *(FPoint p1, FPoint p2)
        {
            var val = ((p1.Val) * (p2.Val)) >> (FRAC_BIT);
            return new FPoint(val, true);
        }

        public static FPoint operator *(FPoint p1, int a)
        {
            return new FPoint(a * p1.Val, true);
        }

        public static FPoint operator *(int a, FPoint p1)
        {
            return p1 * a;
        }

        #endregion

        #region /

        public static FPoint operator /(FPoint p1, FPoint p2)
        {
            if (p2 == FPoint.Zero)
            {
                Logger.Assert(false, "divide by 0");
                return Zero;
            }

            return new FPoint((p1.Val << FRAC_BIT) / (p2.Val), true);
        }

        public static FPoint operator /(FPoint p1, int a)
        {
            if (a == 0)
            {
                Logger.Assert(false, "divide by 0");
                return Zero;
            }

            return new FPoint(p1.Val / a, true);
        }

        #endregion

        #region 开方、绝对值

        internal static FPoint Sqrt(FPoint p1)
        {
            var sq = p1.Val << FRAC_BIT;
            var val = (long) Math.Sqrt(sq);
            return new FPoint(val, true);
        }

        internal static FPoint Abs(FPoint p1)
        {
            return new FPoint(Math.Abs(p1.Val), true);
        }

        #endregion

        #region 比较

        public static bool operator !=(FPoint p1, FPoint p2)
        {
            return p1.Val != p2.Val;
        }

        public static bool operator ==(FPoint p1, FPoint p2)
        {
            return p1.Val == p2.Val;
        }

        public static bool Equals(FPoint p1, FPoint p2)
        {
            return p1.Val == p2.Val;
        }

        public static bool operator <(FPoint p1, FPoint p2)
        {
            return p1.Val < p2.Val;
        }

        public static bool operator <=(FPoint p1, FPoint p2)
        {
            return p1.Val <= p2.Val;
        }

        public static bool operator >=(FPoint p1, FPoint p2)
        {
            return p1.Val >= p2.Val;
        }

        public static bool operator >(FPoint p1, FPoint p2)
        {
            return p1.Val > p2.Val;
        }

        #endregion

        #region Cast

        public int ToInt()
        {
            return (int) (Val >> FRAC_BIT);
        }

        public float ToFloat()
        {
            return Val / (float) (1 << FRAC_BIT);
        }

        public static implicit operator FPoint(int a)
        {
            return new FPoint(a);
        }

        public static implicit operator FPoint(float a)
        {
            return new FPoint(a);
        }

        public override string ToString()
        {
            var tmp = Val / (float) (1 << FRAC_BIT);
            return $"{tmp}F";
        }

        #endregion
    }
}