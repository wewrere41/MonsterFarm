using System;
using Events;
using MainHandlers;
using PlayerBehaviors;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Zenject;

public class PlayerHealthController : IInitializable, IDisposable
{
    #region INJECT

    private readonly PlayerModel _playerModel;
    private readonly PlayerObservables _playerObservables;
    private readonly UISettings _uıSettings;
    private readonly GameObservables _gameObservables;
    private readonly GameStateManager _gameStateManager;
    private readonly SignalBus _signalBus;

    public PlayerHealthController(UISettings uıSettings,
        SignalBus signalBus, GameObservables gameObservables, PlayerModel playerModel,
        PlayerObservables playerObservables, GameStateManager gameStateManager)
    {
        _uıSettings = uıSettings;
        _signalBus = signalBus;
        _gameObservables = gameObservables;
        _playerModel = playerModel;
        _playerObservables = playerObservables;
        _gameStateManager = gameStateManager;
    }

    #endregion

    private readonly CompositeDisposable _compositeDisposable = new();
    private Camera _camera;

    public void Initialize()
    {
        _camera = Camera.main;
        _signalBus.Subscribe<SignalTakeDamage>(x => TakeDamage(x.Collider, x.Damage));
        _signalBus.Subscribe<SignalRevivePlayer>(_ => Revive());
        _signalBus.Subscribe<ISignalResetHealth>(_ => ResetHealth());
        _signalBus.Subscribe<SignalSetActiveItemShop>(x => HealthBarActivate(!x.IsActive));

        _gameObservables.TimeScaleObservable().Where(x => x == 0).Subscribe(x => HealthBarActivate(false))
            .AddTo(_compositeDisposable);
        _gameObservables.TimeScaleObservable().Where(x => x > 0)
            .Subscribe(x => HealthBarActivate(true))
            .AddTo(_compositeDisposable);

        _gameObservables.GameStateLateUpdateObservable.Subscribe(x => HpBarMovement()).AddTo(_compositeDisposable);
    }

    private void TakeDamage(Collider collider, float damage)
    {
        _playerModel.LocalStats.Health -= damage;
        if (_gameStateManager.GameStateReactiveProperty.Value != GameStateManager.GameStates.FailState &&
            _playerModel.LocalStats.Health <= 0)
        {
            Death();
        }

        FillImageByHp();
    }

    private void HpBarMovement()
    {
        var worldToScreen = _camera.WorldToScreenPoint(_playerModel.Position+ new Vector3(0,2.5f,0));
        var parent = _uıSettings.HealthBarFillImage.transform.parent;
        var targetPos = new Vector2(
            Mathf.Lerp(parent.transform.position.x, worldToScreen.x + _uıSettings.HealthBarOffset.x,
                Time.deltaTime * 15),
            Mathf.Lerp(parent.transform.position.y, worldToScreen.y + _uıSettings.HealthBarOffset.y,
                Time.deltaTime * 15));
        parent.position = targetPos;
    }

    private void HealthBarActivate(bool state)
    {
        if (_uıSettings.HealthBarFillImage.transform.parent.gameObject.activeSelf != state)
            _uıSettings.HealthBarFillImage.transform.parent.gameObject.SetActive(state);
    }

    private void FillImageByHp()
    {
        _uıSettings.HealthBarFillImage.fillAmount =
            _playerModel.LocalStats.Health / _playerModel.PlayerStatsSo.Health;
    }

    private void Death()
    {
        _gameStateManager.ChangeState(GameStateManager.GameStates.FailState);
        _playerObservables.SetPlayerState(PlayerState.DIED);
        _playerModel.MeshGO.SetActive(false);
        _gameObservables.SetSkillCanPlayable(false);
        _signalBus.AbstractFire<SignalStopSkill>();
        _signalBus.AbstractFire<SignalPlayerDeath>(
            new(new(PlayerParticleHandler.ParticleType.DEATH)));
    }

    private void Revive()
    {
        HealthBarActivate(true);
        _gameStateManager.ChangeState(GameStateManager.GameStates.InGameState);
        _playerObservables.SetPlayerState(PlayerState.WALK);
        _playerModel.MeshGO.SetActive(true);
    }

    private void ResetHealth()
    {
        _playerModel.LocalStats.Health = _playerModel.PlayerStatsSo.Health;
        FillImageByHp();
    }

    public void Dispose()
    {
        _compositeDisposable?.Dispose();
    }

    [Serializable]
    public class UISettings
    {
        public Image HealthBarFillImage;
        public Vector3 HealthBarOffset;
    }
}