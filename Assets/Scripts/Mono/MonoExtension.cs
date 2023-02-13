using FixMath;
using UnityEngine;

namespace Mono
{
    public static class MonoExtension
    {
        public static Vector2 ToVector2(this FVector2 v)
        {
            return new Vector2(v.x.ToFloat(), v.y.ToFloat());
        }
    }
}