using Logic;

namespace Simulators
{
    /// <summary>
    /// interface of simulator
    /// </summary>
    public interface ISimulator
    {
        public ClientMain Client { get; }

        public void Update(float deltaTime);
    }
}