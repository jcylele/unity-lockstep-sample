using System;
using System.Collections.Generic;
using FixMath;
using Log;

namespace Logic
{
    [Flags]
    public enum DIR
    {
        None,
        Up = 1,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3
    }

    /// <summary>
    ///     逻辑玩家
    /// </summary>
    public class Player : Unit
    {
        private static readonly Dictionary<KeyType, DIR> KeyToDir = new Dictionary<KeyType, DIR>
        {
            {KeyType.Left, DIR.Left},
            {KeyType.Right, DIR.Right},
            {KeyType.Up, DIR.Up},
            {KeyType.Down, DIR.Down},
        };

        private static readonly Dictionary<DIR, FVector2> DirToVector = new Dictionary<DIR, FVector2>
        {
            {DIR.Left, FVector2.Left},
            {DIR.Right, FVector2.Right},
            {DIR.Up, FVector2.Up},
            {DIR.Down, FVector2.Down},
        };

        private DIR mDirection = DIR.None;

        public Player(ClientMain client) : base(client, 0)
        {
        }

        public Player(ClientMain client, UnitSnapshot snapshot) : base(client, snapshot)
        {

        }

        public void OnKey(KeyType key, KeyOpType opType)
        {
            if (!KeyToDir.TryGetValue(key, out var dir))
            {
                Logger.Assert(false, $"Invalid Key: {key}");
            }

            switch (opType)
            {
                case KeyOpType.Pressed:
                    mDirection |= dir;
                    break;
                case KeyOpType.Released:
                    mDirection ^= dir;
                    break;
                default:
                    Logger.Assert(false, $"Invalid KeyOpType: {opType}");
                    break;
            }

            ReCalcDir();
        }

        private void ReCalcDir()
        {
            mMoveDir = FVector2.Zero;

            foreach (var pair in DirToVector)
            {
                if ((mDirection & pair.Key) != DIR.None) mMoveDir += pair.Value;
            }

            mMoveDir = mMoveDir.normalized;
        }

        public void SendOperation(KeyType key, KeyOpType opType)
        {
            var operation = new KeyOperation(key, opType);
            Client.SendToServer(operation);
        }

        public override void Spawn()
        {
            this.mCurPos = FVector2.Zero;
            this.mMoveDir = FVector2.Zero;
        }
    }
}