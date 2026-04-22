using System;
using System.Collections.Generic;
using Core.StateMachine;
using VContainer;
using ZooWorld.Infrastructure.Assets;
using ZooWorld.Infrastructure.Data;
using ZooWorld.Infrastructure.States;

namespace ZooWorld.Infrastructure
{
    public class GameStateMachine : StateMachineBase, IGameStateChanger
    {
        public GameStateMachine(
            CsvAnimalDataLoader csvLoader,
            AnimalDataRegistry dataRegistry,
            IAnimalViewCatalog viewCatalog,
            IObjectResolver rootResolver,
            ISceneReferences sceneReferences)
        {
            _states = new Dictionary<Type, IExitableState>
            {
                [typeof(BootstrapState)]      = new BootstrapState(this, csvLoader, dataRegistry, viewCatalog),
                [typeof(Game_LevelState)]     = new Game_LevelState(this, rootResolver, sceneReferences),
                [typeof(Game_CleanUpState)]   = new Game_CleanUpState(this),
                [typeof(Game_AppQuit_State)]  = new Game_AppQuit_State(),
            };
        }
    }
}
