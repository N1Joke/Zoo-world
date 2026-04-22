using Core.StateMachine;
using UnityEngine;
using VContainer;
using ZooWorld.Gameplay.FX;
using ZooWorld.Gameplay.Level;
using ZooWorld.UI.DeathCounter;
using ZooWorld.Infrastructure;

namespace ZooWorld.Infrastructure.States
{
    public class Game_LevelState : IState
    {
        private readonly IGameStateChanger _stateMachine;
        private readonly IObjectResolver _rootResolver;
        private readonly ISceneReferences _sceneReferences;

        private IScopedObjectResolver _levelScope;
        private LevelInstaller _levelInstaller;

        public Game_LevelState(
            IGameStateChanger stateMachine,
            IObjectResolver rootResolver,
            ISceneReferences sceneReferences)
        {
            _stateMachine = stateMachine;
            _rootResolver = rootResolver;
            _sceneReferences = sceneReferences;
        }

        public void Enter()
        {
            DeathCounterView deathCounterView = _sceneReferences.DeathCounterView;
            FloatingTextView floatingTextPrefab = _sceneReferences.FloatingTextPrefab;
            Transform worldTextParent = _sceneReferences.WorldTextParent;

            LevelScopeInstaller installer = new(deathCounterView, floatingTextPrefab, worldTextParent);

            _levelScope = _rootResolver.CreateScope(installer.Install);
            _levelInstaller = _levelScope.Resolve<LevelInstaller>();
            _levelInstaller.StartGameplay();

            Debug.Log("[Game_LevelState] Level started.");
        }

        public void Exit()
        {
            _levelScope?.Dispose();
            _levelScope = null;
            _levelInstaller = null;
        }
    }
}
