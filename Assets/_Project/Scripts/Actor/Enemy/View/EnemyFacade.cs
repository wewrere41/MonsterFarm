using System;
using EnemyBehavior;
using Events;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

[SelectionBase]
public class EnemyFacade : BaseFacade, IPoolable<Vector3, EnemyCreator.EnemyCreationData, IMemoryPool>, IDisposable
{
    #region INJECT

    private EnemyObservables _enemyObservables;
    private SignalBus _signalBus;
    private EnemyCreator _enemyCreator;
    private IMemoryPool _pool;

    [Inject]
    public void Construct(EnemyObservables enemyObservables, SignalBus signalBus, EnemyCreator enemyCreator)
    {
        _enemyObservables = enemyObservables;
        _signalBus = signalBus;
        _enemyCreator = enemyCreator;
    }

    #endregion

    public EnemyState CurrentState => _enemyObservables.EnemyStateRx.Value;

    public void OnDespawned()
    {
        _pool = null;
    }

    public void OnSpawned(Vector3 pos, EnemyCreator.EnemyCreationData creationData, IMemoryPool pool)
    {
        _pool = pool;
        _enemyCreator.CreateEnemy(creationData);
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, Random.Range(140, 210), 0);
        _enemyObservables.SetState(EnemyState.IDLE);
        _signalBus.Fire<SignalEnemySpawned>();
    }


    public void Dispose()
    {
        _enemyObservables.SetState(EnemyState.NONE);
        ((EnemyModel)_model).NavMeshAgent.enabled = false;
        _pool.Despawn(this);
    }

    public class Factory : PlaceholderFactory<Vector3, EnemyCreator.EnemyCreationData, EnemyFacade>
    {
    }
}