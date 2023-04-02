using FP;

namespace Logic
{
    /// <summary>
    /// Add snapshot support to FRandom
    /// </summary>
    public static class FRandomExtensions
    {
        public static void SaveToSnapShot(this FRandom fRandom, FRandomSnapshot snapshot)
        {
            snapshot.CurSeed = fRandom.GetRandSeed();
        }

        public static void RevertToSnapShot(this FRandom fRandom, FRandomSnapshot snapshot)
        {
            fRandom.SetRandSeed(snapshot.CurSeed);
        }
    }
}
