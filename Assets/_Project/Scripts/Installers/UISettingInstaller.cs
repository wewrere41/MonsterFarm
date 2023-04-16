using System;
using PlayerBehaviors;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class UISettingInstaller : MonoInstaller
{
    [SerializeField, HideLabel, TabGroup("Player")]
    private PlayerSettings _playerSettings;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<PlayerReviveButton>().AsSingle();
        InstallPlayerSettings();
    }

    private void InstallPlayerSettings()
    {
        Container.BindInstance(_playerSettings.MoveHandler).AsSingle();
        Container.BindInstance(_playerSettings.LevelController).AsSingle();
        Container.BindInstance(_playerSettings.SkillController).AsSingle();
        Container.BindInstance(_playerSettings.SkillCooldown).AsSingle();
        Container.BindInstance(_playerSettings.ItemController).AsSingle();
        Container.BindInstance(_playerSettings.SkullHandler).AsSingle();
        Container.BindInstance(_playerSettings.GoldHandler).AsSingle();
        Container.BindInstance(_playerSettings.HealthBar).AsSingle();
        Container.BindInstance(_playerSettings.PlayerResetButton).AsSingle();
    }

    [Serializable]
    private class PlayerSettings
    {
        public PlayerMoveHandler.UISettings MoveHandler;
        public PlayerLevelController.UISettings LevelController;
        public PlayerSkillUpgradeController.UISettings SkillController;
        public PlayerSkillCooldownUIController.UISettings SkillCooldown;
        public PlayerItemShopController.UISettings ItemController;
        public PlayerSkullUIHandler.UISettings SkullHandler;
        public PlayerGoldHandler.UISettings GoldHandler;
        public PlayerHealthController.UISettings HealthBar;
        public PlayerReviveButton.UISettings PlayerResetButton;
    }
}