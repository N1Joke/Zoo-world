using Core.StateMachine;
using UnityEngine;

namespace ZooWorld.Infrastructure.States
{
    public class Game_CleanUpState : IState
    {
        private readonly IGameStateChanger _stateMachine;

        public Game_CleanUpState(IGameStateChanger stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            Debug.Log("[Game_CleanUpState] Cleaning up...");
            _stateMachine.Enter<Game_AppQuit_State>();
        }

        public void Exit() { }
    }
}
