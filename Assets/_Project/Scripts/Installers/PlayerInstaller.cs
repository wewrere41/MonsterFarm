using System;
using PlayerBehaviors;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;
using Zenject;

namespace Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] [TabGroup("Constructor Settings")]
        private Settings _settings;

        [SerializeField] [TabGroup("LocalSettings")]
        private LocalSettings _localSettings;

        public override void InstallBindings()
        {
            Container.Bind(typeof(BaseModel), typeof(PlayerModel)).To<PlayerModel>().AsSingle()
                .WithArguments(_settings.Rigidbody, _settings.Animator, _settings.PlayerStatsSO,_settings.LocalStatHolder);

            InstallPlayerHandlers();
            InstallLocalSettings();
        }


        private void InstallPlayerHandlers()
        {
            Container.Bind<PlayerObservables>().AsSingle();
            Container.BindInterfacesTo<PlayerColliderHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerHealthController>().AsSingle();
            Container.BindInterfacesAndSelfTo<AnimationHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerLevelController>().AsSingle();
            Container.BindInterfacesTo<PlayerMoveHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerAttackSystem>().AsSingle();
            Container.BindInterfacesTo<PlayerSkillExecuter>().AsSingle();
            Container.BindInterfacesTo<PlayerSkillRegistry>().AsSingle();
            Container.BindInterfacesTo<PlayerSkillUpgradeController>().AsSingle();
            Container.BindInterfacesTo<PlayerSkillCooldownUIController>().AsSingle();
            Container.BindInterfacesTo<PlayerEquipmentController>().AsSingle();
            Container.BindInterfacesTo<PlayerItemShopController>().AsSingle();
            Container.BindInterfacesTo<PlayerStatsUpgradeHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerSkullUIHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerGoldHandler>().AsSingle();
            Container.BindInterfacesTo<PlayerParticleHandler>().AsSingle();
        }


        private void InstallLocalSettings()
        {
            Container.BindInstance(_localSettings.AttackSystem).AsSingle();
            Container.BindInstance(_localSettings.ItemController).AsSingle();
            Container.BindInstance(_localSettings.Particle).AsSingle();
        }

        [Serializable]
        [HideLabel]
        private class Settings
        {
            public Rigidbody Rigidbody;
            public Animator Animator;
            public PlayerStatsSO PlayerStatsSO;
            public PlayerModel.LocalStatHolder LocalStatHolder;
        }

        [Serializable, HideLabel]
        private class LocalSettings
        {
            public PlayerAttackSystem.LocalSettings AttackSystem;
            public PlayerEquipmentController.LocalSettings ItemController;
            public PlayerParticleHandler.Settings Particle;
        }
    }
}