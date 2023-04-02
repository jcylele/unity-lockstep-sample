using FP;
using UnityEngine;

namespace Mono
{
    /// <summary>
    /// add extension methods to logic classes
    /// </summary>
    public static class MonoExtension
    {
        public static Vector2 ToVector2(this FVector2 v)
        {
            return new Vector2(v.x.ToFloat(), v.y.ToFloat());
        }
    }
}