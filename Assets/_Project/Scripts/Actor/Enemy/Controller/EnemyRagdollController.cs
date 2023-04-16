using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EnemyBehavior;
using Events;
using UnityEngine;
using Zenject;

public class EnemyRagdollController : IInitializable, IDisposable
{
    #region INJECT

    private readonly EnemyModel _enemyModel;
    private readonly EnemyFacade _enemyFacade;
    private readonly SignalBus _signalBus;
    private readonly LocalSettings _localSettings;

    public EnemyRagdollController(EnemyModel enemyModel, SignalBus signalBus, EnemyFacade enemyFacade,
        LocalSettings localSettings)
    {
        _enemyModel = enemyModel;
        _signalBus = signalBus;
        _enemyFacade = enemyFacade;
        _localSettings = localSettings;
    }

    #endregion

    private readonly CancellationTokenSource _cts = new();
    private Rigidbody[] _rigidbodies;
    private Vector3[] _initialPositions;


    public void Initialize()
    {
        _rigidbodies = _localSettings.RagdollModel.GetComponentsInChildren<Rigidbody>(true);
        _initialPositions = new Vector3[_rigidbodies.Length * 2];

        for (var i = 0; i < _rigidbodies.Length; i++)
        {
            _initialPositions[i] = _rigidbodies[i].transform.localPosition;
            _initialPositions[_rigidbodies.Length + i] = _rigidbodies[i].transform.localEulerAngles;
        }

        _signalBus.Subscribe<SignalEnemySpawned>(x =>
        {
            for (var i = 0; i < _rigidbodies.Length; i++)
            {
                var rigidbody = _rigidbodies[i];
                rigidbody.transform.localPosition = _initialPositions[i];
                rigidbody.transform.localEulerAngles = _initialPositions[_rigidbodies.Length + i];
            }
            
            _localSettings.RagdollModel.SetActive(false);
            _localSettings.BaseModel.SetActive(true);
        });

        _signalBus.Subscribe<SignalEnemyDeath>(x =>
        {
            _localSettings.BaseModel.SetActive(false);
            _localSettings.RagdollModel.SetActive(true);

            DeathOperation(x.HitPosition);
        });
    }

    private async Task DeathOperation(Vector3 hitPosition)
    {
        foreach (var rigidbody in _rigidbodies)
        {
            rigidbody.isKinematic = false;
            rigidbody.AddExplosionForce(15, hitPosition, 5, 0.1f, ForceMode.Impulse);
        }

        await UniTask.Delay(3000, cancellationToken: _cts.Token);

        foreach (var rigidbody in _rigidbodies)
        {
            (rigidbody.velocity, rigidbody.angularVelocity) = (Vector3.zero, Vector3.zero);
            rigidbody.isKinematic = true;
        }

        var currentPosition = _enemyModel.Position.y;
        _enemyModel.GO.transform.DOMoveY(currentPosition - 4, 3).SetEase(Ease.Linear);
        await UniTask.Delay(3000,cancellationToken: _cts.Token);
        _enemyFacade.Dispose();
    }

    [Serializable]
    public class LocalSettings
    {
        public GameObject BaseModel;
        public GameObject RagdollModel;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts?.Dispose();
    }
}