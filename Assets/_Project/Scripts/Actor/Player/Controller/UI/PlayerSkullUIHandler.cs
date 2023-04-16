using System;
using System.Linq;
using Events;
using PlayerBehaviors;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayerSkullUIHandler : IInitializable, IDisposable
{
    #region INJECT

    private readonly PlayerModel _playerModel;
    private readonly UISettings _uiSettings;
    private readonly SignalBus _signalBus;
    private readonly SkullFacade.Factory _skullFactory;
    private readonly CompositeDisposable _compositeDisposable = new();

    public PlayerSkullUIHandler(UISettings uiSettings, SignalBus signalBus, SkullFacade.Factory skullFactory,
        PlayerModel playerModel)
    {
        _uiSettings = uiSettings;
        _signalBus = signalBus;
        _skullFactory = skullFactory;
        _playerModel = playerModel;
    }

    #endregion

    public void Initialize()
    {
        _signalBus.Subscribe<ISignalInstantiateSkull>(OnCollectSkull);

        _signalBus.Subscribe<SignalSkullCountExchange>(x =>
        {
            GainSkull(x.CollectedSkullCount);
            UpdateSkullText();
        });

        _signalBus.Subscribe<SignalPlayerItemUpgrade>(x =>
        {
            if (x.ItemType == ItemTypes.INVENTORY) UpdateSkullText();
        });

        UpdateSkullText();
    }

    private void OnCollectSkull(ISignalInstantiateSkull signal)
    {
        InstantiateSkulls(signal.SkullCount, signal.EnemyPosition);
    }

    private void InstantiateSkulls(int skullCount, Vector3 enemyPosition)
    {
        _skullFactory.Create(
            enemyPosition,
            _uiSettings.SkullImage.transform.position,
            _uiSettings.SkullImage.transform.lossyScale,
            skullCount);
    }


    private void GainSkull(int skullCount)
    {
        _playerModel.PlayerStatsSo.SkullCount =
            (int)Mathf.Clamp(_playerModel.PlayerStatsSo.SkullCount + skullCount, 0,
                PlayerItemHelper.GetItemValue(ItemTypes.INVENTORY));
    }

    private void UpdateSkullText()
    {
        var skullLimit = PlayerItemHelper.GetItemValue(ItemTypes.INVENTORY);
        _uiSettings.SkullCountText.text =
            $"{_playerModel.PlayerStatsSo.SkullCount.ToString()}" +
            $"/{skullLimit}";
        _uiSettings.SkullFillImage.fillAmount =
            _playerModel.PlayerStatsSo.SkullCount /
            skullLimit;
    }


    public void Dispose()
    {
        _compositeDisposable?.Dispose();
    }

    [Serializable]
    public class UISettings
    {
        public Image SkullImage;
        public Image SkullFillImage;
        public TextMeshProUGUI SkullCountText;
    }
}