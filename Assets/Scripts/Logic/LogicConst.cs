using FP;

namespace Logic
{
    /// <summary>
    /// state of the game logic
    /// </summary>
    public enum GameState
    {
        None,
        /// <summary>
        /// local simulator is loading relevant resources
        /// </summary>
        Loading,
        /// <summary>
        /// local simulator is ready, waiting for server to start
        /// </summary>
        Ready,
        /// <summary>
        /// local simulator running
        /// </summary>
        Running,
        /// <summary>
        /// local simulator finished, waiting for server to validate
        /// </summary>
        Finished
    }

    /// <summary>
    /// mode of the game logic, affects behavior of some logic
    /// </summary>
    public enum GamePlayMode
    {
        None,
        /// <summary>
        /// really playing, with network and presentation
        /// </summary>
        Play,
        /// <summary>
        /// replaying with presentation, no network, but with presentation
        /// </summary>
        Replay,
        /// <summary>
        /// replay the game rapidly, without presentation or network
        /// </summary>
        Validate
    }

    /// <summary>
    /// define constants used in logic
    /// </summary>
    public static class LogicConst
    {
        /// <summary>
        /// interval between two frames
        /// </summary>
        public static readonly FPoint FrameInterval = new FPoint(0.033f);

        /// <summary>
        /// half of the map width
        /// </summary>
        public static readonly int MaxX = 640;

        /// <summary>
        /// half of the map height
        /// </summary>
        public static readonly int MaxY = 360;

        /// <summary>
        /// safe radius for spawning from the player
        /// </summary>
        public static readonly int MinSpawnDistance = 100;
    }
}