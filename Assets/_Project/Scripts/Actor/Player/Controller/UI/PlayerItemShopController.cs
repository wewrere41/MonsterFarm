using System;
using System.Linq;
using Events;
using PlayerBehaviors;
using PlayerBehaviors.Helper;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Zenject;
using Object = UnityEngine.Object;

public class PlayerItemShopController : IInitializable
{
    #region INJECT

    private readonly SignalBus _signalBus;
    private readonly UISettings _uıSettings;
    private readonly Settings _settings;
    private readonly AnimationHandler _animationHandler;

    public PlayerItemShopController(SignalBus signalBus, UISettings uıSettings, AnimationHandler animationHandler,
        Settings settings)
    {
        _signalBus = signalBus;
        _uıSettings = uıSettings;
        _animationHandler = animationHandler;
        _settings = settings;
    }

    #endregion

    private readonly ShopItemUI[] _shopItemUIs = new ShopItemUI[5];

    public void Initialize()
    {
        _signalBus.Subscribe<SignalSetActiveItemShop>(x => SetActiveItemPanel(x.IsActive));
        InitializeQuitButton();
        InitializeUpgradeButtons();
    }

    private void InitializeQuitButton()
    {
        _uıSettings.QuitButton.onClick.AddListener(() =>
        {
            SetActiveItemPanel(false);
            _signalBus.Fire(new SignalSetActiveItemShop(false));
        });
    }

    private void InitializeUpgradeButtons()
    {
        for (int i = 0; i < Enum.GetNames(typeof(ItemTypes)).Length; i++)
        {
            var shopItemUI = Object.Instantiate(_settings.ShopItemUI,
                _uıSettings.ShopPanel.transform.GetChild(0).GetChild(0));
            var itemDataSO = PlayerItemHelper.GetItemData((ItemTypes)i);
            var itemType = itemDataSO.ItemType;
            var itemIcon = itemDataSO.ItemSprite;
            var itemName = itemDataSO.ItemName;
            var itemLevel = PlayerItemHelper.GetItemLevel(itemType);
            var itemMaxLevel = itemDataSO.DataArray.Length - 1;
            var goldRequirement = PlayerItemHelper.CheckItemCap(itemType)
                ? "MAX LEVEL"
                : PlayerItemHelper.NextLevelGoldCost(itemType)
                    .ToString();

            _shopItemUIs[i] = shopItemUI;
            shopItemUI.UpgradeButton.onClick.AddListener(() => UpgradeItem(itemType));
            shopItemUI.Initialize(itemType, itemIcon, itemName, itemLevel, itemMaxLevel, goldRequirement);
        }
    }


    private void UpgradeItem(ItemTypes itemType)
    {
        if (PlayerItemHelper.CheckItemCap(itemType))
            return;

        if (PlayerGoldHelper.TryDecreaseGold(PlayerItemHelper.NextLevelGoldCost(itemType)))
        {
            PlayerItemHelper.IncreaseItemLevel(itemType);
            UpdateGoldRequirementText(itemType);
            _signalBus.AbstractFire(new SignalPlayerItemUpgrade(itemType,
                new BaseParticleSignalData(PlayerParticleHandler.ParticleType.ITEM_UPGRADE)));
        }
    }

    private void UpdateGoldRequirementText(ItemTypes itemType)
    {
        var itemLevel = PlayerItemHelper.GetItemLevel(itemType);
        var goldRequirement =
            PlayerItemHelper.CheckItemCap(itemType)
                ? "MAX LEVEL"
                : PlayerItemHelper.NextLevelGoldCost(itemType)
                    .ToString();
        _shopItemUIs.Single(x => x.ItemType == itemType).UpdateUI(itemLevel, goldRequirement);
    }


    #region HELPERS

    private void SetActiveItemPanel(bool isActive)
    {
        _uıSettings.ShopPanel.SetActive(isActive);
        _signalBus.Fire(new SignalJoystickSetActive(!isActive));
        _animationHandler.Play(
            isActive ? Constants.AnimationClips.CINEMACHINE_SHOP : Constants.AnimationClips.CINEMACHINE_INGAME,
            Constants.AnimationLayer.CINEMACHINE);
    }

    #endregion


    [Serializable]
    public class UISettings
    {
        public GameObject ShopPanel;
        public Button QuitButton;
    }

    [Serializable]
    public class Settings
    {
        public ShopItemUI ShopItemUI;
    }
}