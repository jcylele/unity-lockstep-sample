using ProtoBuf;

namespace FixMath
{
    /// <summary>
    /// 定点数2维向量
    /// </summary>
    [ProtoContract]
    public struct FVector2
    {
        [ProtoMember(1)]
        public FPoint x;
        [ProtoMember(2)]
        public FPoint y;

        public FVector2(FPoint x, FPoint y)
        {
            this.x = x;
            this.y = y;
        }

        public static FVector2 Zero => new FVector2(FPoint.Zero, FPoint.Zero);
        public static FVector2 Left => new FVector2(new FPoint(-1), FPoint.Zero);
        public static FVector2 Right => new FVector2(new FPoint(1), FPoint.Zero);
        public static FVector2 Up => new FVector2(FPoint.Zero, new FPoint(1));
        public static FVector2 Down => new FVector2(FPoint.Zero, new FPoint(-1));

        public FPoint sqLength => x * x + y * y;
        public FPoint length => FMath.Sqrt(sqLength);

        public FVector2 normalized
        {
            get
            {
                var sql = this.sqLength;
                if (sql == FPoint.Zero || sql == FPoint.One)
                {
                    return this;
                }

                var len = FMath.Sqrt(sql);
                return this / len;
            }
        }


        public static FVector2 operator +(FVector2 v1, FVector2 v2)
        {
            return new FVector2(v1.x + v2.x, v1.y + v2.y);
        }

        public static FVector2 operator -(FVector2 v1, FVector2 v2)
        {
            return new FVector2(v1.x - v2.x, v1.y - v2.y);
        }

        public static FVector2 operator *(FVector2 v, int i)
        {
            return new FVector2(v.x * i, v.y * i);
        }

        public static FVector2 operator *(FVector2 v, FPoint p)
        {
            return new FVector2(v.x * p, v.y * p);
        }

        public static FVector2 operator /(FVector2 v, int i)
        {
            return new FVector2(v.x / i, v.y / i);
        }

        public static FVector2 operator /(FVector2 v, FPoint i)
        {
            return new FVector2(v.x / i, v.y / i);
        }


        public static FPoint Dot(FVector2 v1, FVector2 v2)
        {
            return v1.x * v2.x + v1.y * v2.y;
        }

        public static bool operator ==(FVector2 v1, FVector2 v2)
        {
            return v1.x == v2.x && v1.y == v2.y;
        }

        public static bool operator !=(FVector2 v1, FVector2 v2)
        {
            return v1.x != v2.x || v1.y != v2.y;
        }

        public override string ToString()
        {
            return $"FVector2({x},{y})";
        }
    }
}