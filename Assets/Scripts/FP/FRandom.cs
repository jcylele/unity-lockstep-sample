using System;
using Logic;

namespace FixMath
{
    /// <summary>
    /// 稳定随机数生成器
    /// </summary>
    public class FRandom : ISnapshot<FRandomSnapshot>
    {
        private ulong mRandSeed;
        private const ulong RandA = 0x5DECEC6D6;
        private const ulong RandC = 0xF;
        private const ulong RandM = ((ulong) 1 << 48);

        public FRandom(ulong randSeed = 0)
        {
            this.mRandSeed = randSeed;
        }

        public FRandom(FRandomSnapshot snapshot)
        {
            this.RevertToSnapShot(snapshot, false);
        }

        public int NextRand()
        {
            mRandSeed = (mRandSeed * RandA + RandC) % RandM;
            return (int)(mRandSeed & int.MaxValue);
        }

        public void SaveToSnapShot(FRandomSnapshot snapshot)
        {
            snapshot.CurSeed = this.mRandSeed;
        }

        public void RevertToSnapShot(FRandomSnapshot snapshot, bool needBase)
        {
            this.mRandSeed = snapshot.CurSeed;
        }
    }
}