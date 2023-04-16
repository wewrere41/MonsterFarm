using System;
using Constants;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using EnemyBehavior;
using Events;
using PlayerBehaviors;
using UniRx;
using UnityEngine;
using Zenject;

public class EnemyMoveHandler : IInitializable, IDisposable
{
    #region INJECT

    private readonly EnemyModel _enemyModel;
    private readonly PlayerFacade _playerFacade;
    private readonly EnemyObservables _enemyObservables;
    private readonly AnimationHandler _animationHandler;
    private readonly SignalBus _signalBus;
    private readonly CompositeDisposable _compositeDisposable = new();

    public EnemyMoveHandler(EnemyModel enemyModel, PlayerFacade playerFacade,
        EnemyObservables enemyObservables, AnimationHandler animationHandler, SignalBus signalBus)
    {
        _enemyModel = enemyModel;
        _playerFacade = playerFacade;
        _enemyObservables = enemyObservables;
        _animationHandler = animationHandler;
        _signalBus = signalBus;
    }

    #endregion

    private readonly CompositeDisposable _takeHitCompositeDisposable = new();
    private Vector3 _startPos;
    private TweenerCore<float, float, FloatOptions> _animationTween;

    public void Initialize()
    {
        _enemyModel.NavMeshAgent.enabled = true;
        _startPos = _enemyModel.Position;

        _signalBus.Subscribe<SignalEnemySpawned>(x =>
        {
            _enemyModel.NavMeshAgent.enabled = true;
            _enemyModel.NavMeshAgent.speed = _enemyModel.EnemyStatsSo.MovementSpeed;
            _startPos = _enemyModel.Position;
        });

        _signalBus.Subscribe<SignalTakeDamage>(_ => TakeDamage());

        _signalBus.Subscribe<SignalEnemyDeath>(_ =>
        {
            DisposeHitDisposables();
            _enemyModel.NavMeshAgent.enabled = false;
        });

        _signalBus.Subscribe<SignalPlayerDeath>(ResetEnemy);
        _signalBus.Subscribe<SignalBaseButton>(ResetEnemy);
        _signalBus.Subscribe<ISignalResetPosition>(ResetPositionAndNavmesh);


        RxWhenMovementSpeedChanges().Subscribe(x => _enemyModel.NavMeshAgent.speed = x)
            .AddTo(_compositeDisposable);


        _enemyObservables.WhileAlive().Subscribe(x =>
        {
            CheckPlayerInRange();
            ChasePlayer();
        }).AddTo(_compositeDisposable);
    }


    private void TakeDamage()
    {
        _enemyModel.NavMeshAgent.speed = 0;
        DisposeHitDisposables();
        Observable.Timer(TimeSpan.FromSeconds(0.1f + Time.deltaTime)).Subscribe(x =>
        {
            var delay = _animationHandler.GetAnimationInterval(AnimationLayer.DEFAULT);
            Observable.Timer(TimeSpan.FromSeconds(delay)).Subscribe(x =>
            {
                _enemyModel.NavMeshAgent.speed = _enemyModel.EnemyStatsSo.MovementSpeed;
                _animationHandler.CrossFadeInFixed(AnimationClips.WALK, AnimationLayer.DEFAULT, 0.1F);
                _enemyObservables.SetState(EnemyState.IDLE);
            }).AddTo(_takeHitCompositeDisposable);
        }).AddTo(_takeHitCompositeDisposable);
    }

    private void ResetEnemy()
    {
        if (_enemyObservables.EnemyStateRx.Value is EnemyState.DIED or EnemyState.NONE)
            return;
        DisposeHitDisposables();
        _animationTween.Kill();
        _enemyModel.NavMeshAgent.SetDestination(_startPos);
        _enemyModel.NavMeshAgent.enabled = false;
        _enemyObservables.SetState(EnemyState.IDLE);
        _animationHandler.SetFloat(AnimationParams.WALK_SPEED, 0);
        _animationHandler.ResetAnimationState(1);
    }

    private void ResetPositionAndNavmesh()
    {
        if (_enemyObservables.EnemyStateRx.Value is EnemyState.DIED or EnemyState.NONE)
            return;
        _enemyModel.Position = _startPos;
        _enemyModel.NavMeshAgent.enabled = true;
        _enemyModel.NavMeshAgent.speed = _enemyModel.EnemyStatsSo.MovementSpeed;
    }

    #region MOVEMENT

    private void CheckPlayerInRange()
    {
        var distance = _enemyObservables.GetPlayerDistance;
        if (_enemyObservables.EnemyStateRx.Value is EnemyState.IDLE && distance < _enemyModel.LocalStats.FollowRange)
        {
            _enemyObservables.SetState(EnemyState.CHASE);

            _animationTween = DOTween.To(() => _animationHandler.GetFloat(AnimationParams.WALK_SPEED),
                x => _animationHandler.SetFloat(AnimationParams.WALK_SPEED, x), 1,
                0.25f);
        }
        else if (_enemyObservables.EnemyStateRx.Value == EnemyState.CHASE &&
                 distance > _enemyModel.LocalStats.FollowRange)
        {
            _enemyObservables.SetState(EnemyState.IDLE);

            _enemyModel.NavMeshAgent.SetDestination(_enemyModel.Position);
            _animationTween = DOTween.To(() => _animationHandler.GetFloat(AnimationParams.WALK_SPEED),
                x => _animationHandler.SetFloat(AnimationParams.WALK_SPEED, x), 0,
                0.25f);
        }
        else if ((_enemyObservables.EnemyStateRx.Value
                  & EnemyState.MOVEBACK) == 0 && distance < 1f)
        {
            _enemyModel.NavMeshAgent.SetDestination(_enemyModel.Position);
            _enemyObservables.SetState(EnemyState.MOVEBACK);
        }
    }

    private void ChasePlayer()
    {
        if (_enemyObservables.EnemyStateRx.Value == EnemyState.CHASE)
        {
            _enemyModel.NavMeshAgent.SetDestination(_playerFacade.transform.position);
        }

        if (_enemyObservables.EnemyStateRx.Value == EnemyState.MOVEBACK)
        {
            if (_enemyObservables.GetPlayerDistance > 1.4f)
            {
                _enemyObservables.SetState(EnemyState.IDLE);
            }
            else
            {
                _enemyModel.GO.transform.rotation = Quaternion.Slerp(_enemyModel.GO.transform.rotation,
                    Quaternion.LookRotation(_playerFacade.transform.position - _enemyModel.Position),
                    Time.deltaTime * 6);
                _enemyModel.NavMeshAgent.Move(-0.03f * (_playerFacade.transform.position - _enemyModel.Position)
                    .normalized);
            }
        }
    }

    #endregion

    private void DisposeHitDisposables() => _takeHitCompositeDisposable.Clear();

    #region RX

    private IObservable<float> RxWhenMovementSpeedChanges() =>
        this.ObserveEveryValueChanged(x => _enemyModel.EnemyStatsSo.MovementSpeed);

    #endregion

    public void Dispose()
    {
        _compositeDisposable?.Dispose();
        _takeHitCompositeDisposable?.Dispose();
    }
}