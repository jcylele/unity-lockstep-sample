namespace FP
{
    /// <summary>
    /// deterministic random number generator
    /// </summary>
    public class FRandom
    {
        private ulong mRandSeed;
        private const ulong RandA = 0x5DECEC6D6;
        private const ulong RandC = 0xF;
        private const ulong RandM = ((ulong) 1 << 48);

        public FRandom(ulong randSeed = 0)
        {
            this.mRandSeed = randSeed;
        }

        public int NextRand()
        {
            mRandSeed = (mRandSeed * RandA + RandC) % RandM;
            return (int)(mRandSeed & int.MaxValue);
        }

        /// <summary>
        /// set rand seed, don't use it unless you're sure
        /// </summary>
        /// <param name="randSeed">new rand seed</param>
        public void SetRandSeed(ulong randSeed)
        {
            mRandSeed = randSeed;
        }

        public ulong GetRandSeed()
        {
            return this.mRandSeed;
        }
    }
}