using System;
using Events;
using UniRx;
using UnityEngine.UI;
using Zenject;

public class PlayerReviveButton : IInitializable, IDisposable
{
    #region INJECT

    private readonly UISettings _uıSettings;
    private readonly SignalBus _signalBus;

    public PlayerReviveButton(UISettings uıSettings, SignalBus signalBus)
    {
        _uıSettings = uıSettings;
        _signalBus = signalBus;
    }

    #endregion

    private readonly CompositeDisposable _compositeDisposable = new();

    public void Initialize()
    {
        _uıSettings.PlayerResetButton.onClick.AddListener(() => _signalBus.AbstractFire<SignalRevivePlayer>());
        _uıSettings.BaseButton.onClick.AddListener(() =>
        {
            _signalBus.AbstractFire<SignalBaseButton>();
            _signalBus.AbstractFire<SignalStopSkill>();
        });
    }

    public void Dispose()
    {
        _compositeDisposable?.Dispose();
    }

    [Serializable]
    public class UISettings
    {
        public Button PlayerResetButton;
        public Button BaseButton;
    }
}