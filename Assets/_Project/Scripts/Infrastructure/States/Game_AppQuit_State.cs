using Core.StateMachine;
using UnityEngine;

namespace ZooWorld.Infrastructure.States
{
    public class Game_AppQuit_State : IState
    {
        public void Enter()
        {
            Debug.Log("[Game_AppQuit_State] Quitting...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void Exit() { }
    }
}
