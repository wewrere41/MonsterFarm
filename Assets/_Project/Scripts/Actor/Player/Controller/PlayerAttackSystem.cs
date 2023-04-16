using System;
using System.Threading;
using System.Threading.Tasks;
using Constants;
using Cysharp.Threading.Tasks;
using Events;
using MainHandlers;
using MoreMountains.NiceVibrations;
using PlayerBehaviors;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utilities.Helper;
using Zenject;

public class PlayerAttackSystem : IInitializable, IDisposable
{
    #region INJECT

    private readonly PlayerModel _playerModel;
    private readonly GameObservables _gameObservables;
    private readonly PlayerObservables _playerObservables;
    private readonly AnimationHandler _animationHandler;
    private readonly SignalBus _signalBus;
    private readonly LocalSettings _localSettings;
    private readonly CompositeDisposable _disposables = new();

    public PlayerAttackSystem(PlayerModel playerModel, GameObservables gameObservables,
        AnimationHandler animationHandler, SignalBus signalBus, LocalSettings localSettings,
        PlayerObservables playerObservables)
    {
        _playerModel = playerModel;
        _gameObservables = gameObservables;
        _animationHandler = animationHandler;
        _signalBus = signalBus;
        _localSettings = localSettings;
        _playerObservables = playerObservables;
    }

    #endregion

    private readonly Collider[] _enemies = new Collider[4];

    private CancellationTokenSource _ctsAttack = new();
    private IDisposable _rxSkillDisableTimer;
    private IDisposable _rxWhenEnemyDeathDisposable;
    private Collider _activeCollider;
    private Vector3 _defaultColliderCenter;


    public void Initialize()
    {
        InitializeActiveCollider();
        InitializeWeaponTrigger();

        _signalBus.Subscribe<SignalPlayerItemUpgrade>(x =>
        {
            if (x.ItemType == ItemTypes.WEAPON) InitializeActiveCollider();
        });
        _signalBus.Subscribe<SignalSetActiveCollider>(x => _activeCollider.enabled = x.IsActive);
        _signalBus.Subscribe<SignalPlayerDeath>(x => CancelAttackCts());

        RxAttackSpeedChanged().Subscribe(SetAttackAnimationSpeed).AddTo(_disposables);
        RxWhenSkillState().Subscribe(x => CancelAttackCts()).AddTo(_disposables);
        RxWhenHaveEnemy().Subscribe(_ =>
            {
                _rxSkillDisableTimer?.Dispose();
                _gameObservables.SetSkillCanPlayable(true);
            })
            .AddTo(_disposables);


        _playerObservables.WhileAlive().Subscribe(x =>
        {
            CheckEnemyInRange();
            AttackToEnemy();
        }).AddTo(_disposables);

        #region LOG

/*
#if LOG
        this.ObserveEveryValueChanged(x => _playerObservables.PlayerState)
            .Subscribe(x => Debug.Log($"<color=GREEN>{x}</color>")).AddTo(_disposables);

        this.ObserveEveryValueChanged(x => _playerObservables.TargetEnemy).Skip(1)
            .Subscribe(x => { Debug.Log($"<color=RED>{(x == null ? "NULL" : x)}</color>"); })
            .AddTo(_disposables);
#endif

        */

        #endregion
    }


    private void CheckEnemyInRange()
    {
        if (_playerObservables.TargetEnemy == null)
        {
            var enemySize = Physics.OverlapSphereNonAlloc(_playerModel.Position, 2f, _enemies, Layer.ENEMY);
            for (var i = 0; i < enemySize; i++)
            {
                var enemy = _enemies[i];
                var lookPercent = MathfUtilities.CalculateDotProduct(_playerModel.GO.transform, enemy.transform);
                if (lookPercent >= 0.5f)
                {
                    enemy.TryGetComponent(out EnemyFacade enemyFacade);
                    _playerObservables.TargetEnemy = enemyFacade;
                    _rxWhenEnemyDeathDisposable?.Dispose();
                    _rxWhenEnemyDeathDisposable =
                        RxWhenEnemyDeath().Subscribe(_ => SetNullTargetEnemy()).AddTo(_disposables);

                    return;
                }
            }
        }
        else
        {
            var distance =
                Vector3.Distance(_playerModel.Position, _playerObservables.TargetEnemy.transform.position);
            if (distance > 5)
            {
                SetNullTargetEnemy();
            }
        }
    }

    private async Task AttackToEnemy()
    {
        if (_playerObservables.TargetEnemy)
        {
            if ((_playerObservables.PlayerState & PlayerState.ATTACKABLES) != 0)
            {
                var attackTransitionDuration = CalculateTransitionDuration();
                var oldTransitionDuration = _animationHandler.GetAnimationTransitionDuration(AnimationLayer.UPPER);

                _playerObservables.SetPlayerState(PlayerState.ATTACK);
                CreateAttackCts();


                await Task.Delay(TimeSpan.FromSeconds(oldTransitionDuration), _ctsAttack.Token);

                _animationHandler.CrossFadeInFixed(AnimationClips.ATTACK, AnimationLayer.UPPER,
                    attackTransitionDuration);

                await Task.Delay(TimeSpan.FromSeconds(attackTransitionDuration + Time.deltaTime), _ctsAttack.Token);

                await UniTask.WaitUntil(() => _animationHandler.GetNormalizedTime(AnimationLayer.UPPER) >= 0.3f,
                    PlayerLoopTiming.Update, _ctsAttack.Token);

                _activeCollider.enabled = true;

                await UniTask.WaitUntil(() => _animationHandler.GetNormalizedTime(AnimationLayer.UPPER) >= 0.5f,
                    PlayerLoopTiming.Update, _ctsAttack.Token);

                var newCenter = _defaultColliderCenter + new Vector3(0, 0.25f, 0);
                ((BoxCollider)_activeCollider).center = newCenter;
                _signalBus.AbstractFire(new SignalPlayerAttack(HapticTypes.MediumImpact));

                await UniTask.WaitUntil(() => _animationHandler.GetNormalizedTime(AnimationLayer.UPPER) >= 0.6f,
                    PlayerLoopTiming.Update, _ctsAttack.Token);

                ((BoxCollider)_activeCollider).center = _defaultColliderCenter;

                await UniTask.WaitUntil(() => _animationHandler.GetNormalizedTime(AnimationLayer.UPPER) >= 0.8f,
                    PlayerLoopTiming.Update, _ctsAttack.Token);

                _activeCollider.enabled = false;

                await UniTask.WaitUntil(() => _animationHandler.GetNormalizedTime(AnimationLayer.UPPER) >= 1f,
                    PlayerLoopTiming.Update, _ctsAttack.Token);

                if (_playerObservables.TargetEnemy != null)
                {
                    var dot = Vector3.Dot(_playerModel.GO.transform.forward,
                        (_playerObservables.TargetEnemy.transform.position - _playerModel.Position).normalized);

                    if (dot < 0.5f)
                    {
                        SetNullTargetEnemy();
                        _animationHandler.CrossFadeInFixed(AnimationClips.NONE, AnimationLayer.UPPER, 0.2f);
                        _playerObservables.SetPlayerState(PlayerState.WALK);
                    }
                    else
                    {
                        _playerObservables.SetPlayerState(PlayerState.ATTACKEND);
                    }
                }
                else
                {
                    _animationHandler.CrossFadeInFixed(AnimationClips.NONE, AnimationLayer.UPPER, 0.2f);
                    _playerObservables.SetPlayerState(PlayerState.WALK);
                }
            }
        }
    }

    private void SetNullTargetEnemy()
    {
        _playerObservables.TargetEnemy = null;
        _rxSkillDisableTimer = RxSkillDisableTimer();
        _rxWhenEnemyDeathDisposable?.Dispose();
    }

    private void SetAttackAnimationSpeed(float speed) =>
        _animationHandler.SetFloat(AnimationParams.ATTACK_SPEED, speed);


    #region CTS

    private void CreateAttackCts()
    {
        if (_ctsAttack.IsCancellationRequested)
            _ctsAttack = new();
    }

    private void CancelAttackCts()
    {
        ((BoxCollider)_activeCollider).center = _defaultColliderCenter;
        if (!_ctsAttack.IsCancellationRequested)
        {
            _ctsAttack.Cancel();
            _ctsAttack?.Dispose();
        }
    }

    #endregion

    #region RX

    private IObservable<float> RxAttackSpeedChanged() =>
        this.ObserveEveryValueChanged(x => _playerModel.BaseStatsSo.AttackSpeed);

    private IObservable<EnemyFacade> TargetEnemyObservable() =>
        this.ObserveEveryValueChanged(x => _playerObservables.TargetEnemy);


    private IObservable<EnemyFacade> RxWhenHaveEnemy() => TargetEnemyObservable().Where(x => x != null);


    private IObservable<EnemyState> RxWhenEnemyDeath() =>
        this.ObserveEveryValueChanged(x => _playerObservables.TargetEnemy.CurrentState)
            .Where(x => x == EnemyState.DIED);

    private IObservable<PlayerState> RxWhenSkillState() =>
        this.ObserveEveryValueChanged(x => _playerObservables.PlayerState)
            .Where(x => x == PlayerState.SKILL);

    private IDisposable RxSkillDisableTimer()
    {
        return Observable.Timer(TimeSpan.FromSeconds(3))
            .Subscribe(_ => _gameObservables.SetSkillCanPlayable(false))
            .AddTo(_disposables);
    }

    #endregion

    #region SIGNAL

    private void InitializeActiveCollider()
    {
        _activeCollider = _localSettings.WeaponParent.GetChild(PlayerItemHelper.GetItemLevel(ItemTypes.WEAPON))
            .GetComponent<Collider>();
        _defaultColliderCenter = ((BoxCollider)_activeCollider).center;
    }

    private void InitializeWeaponTrigger()
    {
        _localSettings.WeaponParent.OnTriggerEnterAsObservable().Subscribe(enemy =>
        {
            var damage = _playerModel.PlayerStatsSo.AttackDamage;
            enemy.TryGetComponent(out CollisionBridge collisionBridge);
            collisionBridge.TakeDamage(_activeCollider, damage);
        }).AddTo(_disposables);
    }

    #endregion

    #region HELPER

    private float CalculateTransitionDuration() =>
        _playerObservables.PlayerState == PlayerState.ATTACKEND ? 0 : 0.1f;

    #endregion

    public void Dispose()
    {
        _disposables?.Dispose();
        _rxWhenEnemyDeathDisposable?.Dispose();
        CancelAttackCts();
    }

    [Serializable]
    public class LocalSettings
    {
        public Transform WeaponParent;
    }
}