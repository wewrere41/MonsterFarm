using System;
using MainHandlers;
using PlayerBehaviors.Helper;
using UnityEngine;
using Utilities;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [Inject] private GameSettingsInstaller.PoolObjects _poolObjects;
        [SerializeField] private Settings _settings;

        public override void InstallBindings()
        {
            GameSignalsInstaller.Install(Container);
            InstallDebugger();
            InstallMainBehaviors();
            InstallPlayerHelpers();
            InstallUtilities();
            InstallPools();
            InstallSettings();
        }

        private void InstallDebugger()
        {
            Container.BindInterfacesTo<SRDebugContainer>().AsSingle().NonLazy();
        }

        private void InstallMainBehaviors()
        {
            Container.BindInterfacesAndSelfTo<GameStateManager>().AsSingle();
            Container.Bind<GameObservables>().AsSingle();
            Container.BindInterfacesTo<LevelProgressionController>().AsSingle();
        }

        private void InstallPlayerHelpers()
        {
            Container.Instantiate<PlayerStatHelper>();
            Container.Instantiate<PlayerItemHelper>();
            Container.Instantiate<PlayerSkillHelper>();
            Container.Instantiate<PlayerGoldHelper>();
            Container.Instantiate<PlayerSkullHelper>();
        }

        private void InstallPools()
        {
            Container.BindFactory<Vector3, EnemyCreator.EnemyCreationData, EnemyFacade, EnemyFacade.Factory>()
                .FromPoolableMemoryPool<Vector3, EnemyCreator.EnemyCreationData, EnemyFacade, EnemyPool>(b => b
                    .WithInitialSize(100)
                    .FromComponentInNewPrefab(_poolObjects.Enemy)
                    .UnderTransformGroup("EnemyPool"));


            Container.BindFactory<Vector3, Vector3, Vector3, int, SkullFacade, SkullFacade.Factory>()
                .FromPoolableMemoryPool<Vector3, Vector3, Vector3, int, SkullFacade, SkullPool>(b => b
                    .WithInitialSize(30)
                    .FromComponentInNewPrefab(_poolObjects.Skull)
                    .UnderTransformGroup("SkullPool"));


            Container.BindFactory<Vector3, Vector3, Vector3, GoldFacade, GoldFacade.Factory>()
                .FromPoolableMemoryPool<Vector3, Vector3, Vector3, GoldFacade, GoldPool>(b => b
                    .WithInitialSize(30)
                    .FromComponentInNewPrefab(_poolObjects.Gold)
                    .UnderTransformGroup("GoldPool"));
        }

        private void InstallUtilities()
        {
            Container.BindInterfacesTo<AudioAndHapticManager>().AsSingle();
        }

        private void InstallSettings()
        {
            Container.BindInstance(_settings.ProgressionController);
        }

        [Serializable]
        private class Settings
        {
            public LevelProgressionController.Settings ProgressionController;
        }
    }
}