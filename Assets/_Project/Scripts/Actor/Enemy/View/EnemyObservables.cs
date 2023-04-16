using System;
using EnemyBehavior;
using MainHandlers;
using PlayerBehaviors;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utilities.Helper;

public class EnemyObservables
{
    #region INJECT

    private readonly EnemyModel _enemyModel;
    private readonly GameObservables _gameObservables;
    private readonly PlayerFacade _playerFacade;
    private readonly EnemyCreator.LocalSettings _enemyCreatorSettings;

    public EnemyObservables(EnemyModel enemyModel, GameObservables gameObservables, PlayerFacade playerFacade,
        EnemyCreator.LocalSettings enemyCreator)
    {
        _enemyModel = enemyModel;
        _gameObservables = gameObservables;
        _playerFacade = playerFacade;
        _enemyCreatorSettings = enemyCreator;
    }

    #endregion


    private IObservable<Collider> EnemyWeaponTriggerEnterObservable =>
        _enemyCreatorSettings.BaseWeaponParent.OnTriggerEnterAsObservable();

    public float GetPlayerDistance
    {
        get
        {
            var enemyPosition = _enemyModel.Position;
            var playerPosition = _playerFacade.transform.position;
            return MathfUtilities.FastDistance(enemyPosition, playerPosition);
        }
    }

    #region STATE

    public readonly ReactiveProperty<EnemyState> EnemyStateRx = new(EnemyState.IDLE);
    public void SetState(EnemyState state) => EnemyStateRx.Value = state;

    #endregion

    #region COMMON

    public IObservable<GameStateManager.GameStates> WhileAlive() =>
        _gameObservables.GameStateUpdateObservable.Where(x =>
            EnemyStateRx.Value is not EnemyState.DIED and not EnemyState.NONE &&
            x == GameStateManager.GameStates.InGameState);

    public IObservable<Collider> WhenHitToPlayer() => EnemyWeaponTriggerEnterObservable.Where(x =>
        EnemyStateRx.Value != EnemyState.DIED && x.CompareTag("Player"));

    #endregion
}