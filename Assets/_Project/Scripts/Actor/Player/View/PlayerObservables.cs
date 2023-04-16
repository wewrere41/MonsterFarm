using System;
using MainHandlers;
using PlayerBehaviors;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class PlayerObservables
{
    #region INJECT

    private readonly PlayerModel _player;
    private readonly GameObservables _gameObservables;

    public PlayerObservables(PlayerModel player, GameObservables gameObservables)
    {
        _player = player;
        _gameObservables = gameObservables;
    }

    #endregion

    #region COLLISION

    public IObservable<Collision> PlayerCollisionEnterObservable => _player.GO.OnCollisionEnterAsObservable();
    public IObservable<Collision> PlayerCollisionStayObservable => _player.GO.OnCollisionStayAsObservable();
    public IObservable<Collision> PlayerCollisionExitObservable => _player.GO.OnCollisionExitAsObservable();
    public IObservable<Collider> PlayerTriggerStayObservable => _player.GO.OnTriggerStayAsObservable();
    public IObservable<Collider> PlayerTriggerExitObservable => _player.GO.OnTriggerExitAsObservable();
    public IObservable<Collider> PlayerTriggerEnterObservable => _player.GO.OnTriggerEnterAsObservable();

    #endregion

    public IObservable<GameStateManager.GameStates> WhileAlive() =>
        _gameObservables.GameStateUpdateObservable.Where(x =>
            _playerStateRx.Value != PlayerState.DIED && x == GameStateManager.GameStates.InGameState);

    #region PLAYER STATE

    private readonly ReactiveProperty<PlayerState>
        _playerStateRx = new(PlayerState.WALK);

    public PlayerState PlayerState => _playerStateRx.Value;

    public void SetPlayerState(PlayerState state)
    {
        _playerStateRx.Value = state;
    }

    #endregion

    private readonly ReactiveProperty<EnemyFacade> _targetEnemy = new(null);

    public EnemyFacade TargetEnemy
    {
        get => _targetEnemy.Value;
        set => _targetEnemy.Value = value;
    }
}