using System;
using System.Threading;
using System.Threading.Tasks;
using Constants;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EnemyBehavior;
using Events;
using PlayerBehaviors;
using UniRx;
using UnityEngine;
using Utilities.Helper;
using Zenject;
using Random = UnityEngine.Random;

public class EnemyAttackSystem : IInitializable, IDisposable
{
    #region INJECT

    private readonly EnemyModel _enemyModel;
    private readonly EnemyObservables _enemyObservables;
    private readonly PlayerFacade _playerFacade;
    private readonly AnimationHandler _animationHandler;
    private readonly SignalBus _signalBus;
    private readonly EnemyCreator.LocalSettings _enemyCreatorSettings;
    private readonly CompositeDisposable _disposables = new();

    public EnemyAttackSystem(EnemyModel enemyModel, EnemyObservables enemyObservables,
        PlayerFacade playerFacade, AnimationHandler animationHandler, SignalBus signalBus,
        EnemyCreator.LocalSettings enemyCreatorSettings)
    {
        _enemyModel = enemyModel;
        _enemyObservables = enemyObservables;
        _playerFacade = playerFacade;
        _animationHandler = animationHandler;
        _signalBus = signalBus;
        _enemyCreatorSettings = enemyCreatorSettings;
    }

    #endregion

    private CancellationTokenSource _ctsAttack = new();
    private Collider _activeCollider;


    public void Initialize()
    {
        InitializeActiveCollider();
        _signalBus.Subscribe<SignalEnemyDeath>(_ => CancelAttackCts());
        _signalBus.Subscribe<SignalTakeDamage>(_ => CancelAttack());
        _signalBus.Subscribe<SignalPlayerDeath>(_ => CancelAttack());

        RxAttackSpeedChanged().Subscribe(SetAttackAnimationSpeed).AddTo(_disposables);

        _enemyObservables.WhenHitToPlayer().Subscribe(_ => HitToPlayer()).AddTo(_disposables);

        _enemyObservables.WhileAlive().Subscribe(x =>
        {
            CheckPlayerInAttackRange();
            RotateToPlayer();
        }).AddTo(_disposables);
    }


    private void CheckPlayerInAttackRange()
    {
        if ((_enemyObservables.EnemyStateRx.Value &
             (EnemyState.ATTACKSTATES | EnemyState.TAKINGDAMAGE | EnemyState.MOVEBACK)) == 0 &&
            _enemyObservables.GetPlayerDistance <= _enemyModel.LocalStats.AttackRange)
        {
            _enemyObservables.SetState(EnemyState.CANATTACK);

            _enemyModel.NavMeshAgent.SetDestination(_enemyModel.Position);
            DOTween.To(() => _animationHandler.GetFloat(AnimationParams.WALK_SPEED),
                x => _animationHandler.SetFloat(AnimationParams.WALK_SPEED, x), 0,
                0.25f);
        }
    }

    private void RotateToPlayer()
    {
        if (_enemyObservables.EnemyStateRx.Value is EnemyState.CANATTACK or EnemyState.ATTACKEND)
        {
            var lookPercent = MathfUtilities.CalculateDotProduct(_enemyModel.GO.transform, _playerFacade.transform);
            if (lookPercent <= 0.95f)
            {
                _enemyModel.GO.transform.rotation = Quaternion.Slerp(_enemyModel.GO.transform.rotation,
                    Quaternion.LookRotation(_playerFacade.transform.position - _enemyModel.Position),
                    Time.deltaTime * 6);
            }
            else
            {
                AttackToPlayer();
            }
        }
    }


    private async Task AttackToPlayer()
    {
        if (_enemyObservables.EnemyStateRx.Value != EnemyState.ATTACK)
        {
            _enemyObservables.SetState(EnemyState.ATTACK);

            CreateAttackCts();

            var attackTransitionDuration = CalculateTransitionDuration();
            var oldTransitionDuration = _animationHandler.GetAnimationTransitionDuration(AnimationLayer.UPPER);


            await Task.Delay(TimeSpan.FromSeconds(oldTransitionDuration), _ctsAttack.Token);

            _animationHandler.CrossFadeInFixed(AnimationClips.ATTACK, AnimationLayer.UPPER,
                attackTransitionDuration);

            await Task.Delay(TimeSpan.FromSeconds(attackTransitionDuration + Time.deltaTime), _ctsAttack.Token);

            await UniTask.WaitUntil(() => _animationHandler.GetNormalizedTime(AnimationLayer.UPPER) >= 0.3f,
                PlayerLoopTiming.Update, _ctsAttack.Token);

            _activeCollider.enabled = true;

            await UniTask.WaitUntil(() => _animationHandler.GetNormalizedTime(AnimationLayer.UPPER) >= 0.6f,
                PlayerLoopTiming.Update, _ctsAttack.Token);

            _activeCollider.enabled = false;

            await UniTask.WaitUntil(() => _animationHandler.GetNormalizedTime(AnimationLayer.UPPER) >= 1f,
                PlayerLoopTiming.Update, _ctsAttack.Token);

            if (_enemyObservables.GetPlayerDistance > _enemyModel.LocalStats.AttackRange)
            {
                _animationHandler.CrossFadeInFixed(AnimationClips.NONE, AnimationLayer.UPPER, 0.1f);
                _enemyObservables.SetState(EnemyState.IDLE);
            }
            else
            {
                _enemyObservables.SetState(EnemyState.ATTACKEND);
            }
        }
    }

    private void HitToPlayer()
    {
        var damage = _enemyModel.EnemyStatsSo.AttackDamage;
        _playerFacade.TryGetComponent(out CollisionBridge collisionBridge);
        collisionBridge.TakeDamage(_activeCollider, damage);
    }

    private void InitializeActiveCollider()
    {
        _activeCollider = _enemyCreatorSettings.BaseWeaponParent.GetComponentInChildren<Collider>();
    }

    private void CancelAttack()
    {
        _activeCollider.enabled = false;
        CancelAttackCts();
    }

    private void SetAttackAnimationSpeed(float speed)
    {
        _animationHandler.SetFloat(AnimationParams.ATTACK_SPEED, speed * Random.Range(1, 1.2f));
    }

    #region CTS

    private void CreateAttackCts()
    {
        if (_ctsAttack.IsCancellationRequested)
            _ctsAttack = new();
    }

    private void CancelAttackCts()
    {
        if (!_ctsAttack.IsCancellationRequested)
        {
            _ctsAttack.Cancel();
            _ctsAttack?.Dispose();
        }
    }

    #endregion

    #region RX

    private IObservable<float> RxAttackSpeedChanged() =>
        this.ObserveEveryValueChanged(x => _enemyModel.EnemyStatsSo.AttackSpeed);

    #endregion

    #region HELPER

    private float CalculateTransitionDuration() =>
        _enemyObservables.EnemyStateRx.Value != EnemyState.ATTACKEND ? 0.1f : 0;

    #endregion


    public void Dispose()
    {
        _disposables?.Dispose();
        CancelAttackCts();
    }
}