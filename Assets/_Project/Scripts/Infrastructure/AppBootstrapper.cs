using VContainer.Unity;
using ZooWorld.Infrastructure.States;

namespace ZooWorld.Infrastructure
{
    // First entry point after the root scope is built — kicks off the state machine.
    public class AppBootstrapper : IStartable
    {
        private readonly GameStateMachine _gameStateMachine;

        public AppBootstrapper(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public void Start()
        {
            _gameStateMachine.Enter<BootstrapState>();
        }
    }
}
