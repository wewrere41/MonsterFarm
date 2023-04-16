using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
    [InlineEditor]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        [TabGroup("PlayerSettings")] [HideLabel] [SerializeField]
        private PlayerSettings Player;
        
        [SerializeField] private WorldStatsSO worldStatsSo;

        [SerializeField] private PoolObjects _poolObjects;

        public override void InstallBindings()
        {
            InstallWorldSettings();
            InstallPlayerSettings();
            InstallPoolSettings();
        }

        private void InstallWorldSettings()
        {
            Container.BindInstance(worldStatsSo).AsSingle();
        }

        private void InstallPlayerSettings()
        {
            Container.BindInstance(Player.PlayerData).AsSingle();
            Container.BindInstance(Player.ItemShop).AsSingle();
        }

        private void InstallPoolSettings()
        {
            Container.BindInstance(_poolObjects).AsSingle();
        }

        [Serializable]
        private class PlayerSettings
        {
            public PlayerDataSO PlayerData;
            public PlayerItemShopController.Settings ItemShop;
        }


        [Serializable]
        public class PoolObjects
        {
            public EnemyFacade Enemy;
            public SkullFacade Skull;
            public GoldFacade Gold;
        }
    }

    public class EnemyPool : MonoPoolableMemoryPool<Vector3, EnemyCreator.EnemyCreationData, IMemoryPool, EnemyFacade>
    {
    }

    public class SkullPool : MonoPoolableMemoryPool<Vector3, Vector3, Vector3, int, IMemoryPool, SkullFacade>
    {
    }

    public class GoldPool : MonoPoolableMemoryPool<Vector3, Vector3, Vector3, IMemoryPool, GoldFacade>
    {
    }
}