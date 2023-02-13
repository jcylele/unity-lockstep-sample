using Logic;

namespace Mono
{
    public interface ISimulator
    {
        public ClientMain Client { get; }

        public void Update(float deltaTime);
    }
}