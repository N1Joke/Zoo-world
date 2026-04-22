namespace Core.StateMachine
{
    public interface IState : IExitableState
    {
        void Enter();
    }
}
