using ProtoBuf;

namespace FP
{
    /// <summary>
    /// fixpoint version of Vector2, for deterministic calculation
    /// </summary>
    [ProtoContract]
    public struct FVector2
    {
        public bool Equals(FVector2 other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }

        public override bool Equals(object obj)
        {
            return obj is FVector2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x.GetHashCode() * 397) ^ y.GetHashCode();
            }
        }

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

        /// <summary>
        /// square length, use this instead of Length if you only need to compare length
        /// </summary>
        public FPoint SqLength => x * x + y * y;
        public FPoint Length => FMath.Sqrt(SqLength);

        /// <summary>
        /// normalized vector
        /// </summary>
        public FVector2 Normalized
        {
            get
            {
                var sql = this.SqLength;
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