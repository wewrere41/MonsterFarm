using System;
using System.Threading.Tasks;
using Constants;
using EnemyBehavior;
using Events;
using PlayerBehaviors;
using UniRx;
using UnityEngine;
using Utilities;
using Zenject;

public class EnemyHealthSystem : IInitializable, IDisposable
{
    #region INJECT

    private readonly EnemyObservables _enemyObservables;
    private readonly AnimationHandler _animationHandler;
    private readonly EnemyModel _enemyModel;
    private readonly LocalSettings _localSettings;
    private readonly SignalBus _signalBus;
    private readonly CompositeDisposable _disposables = new();

    public EnemyHealthSystem(EnemyObservables enemyObservables,
        AnimationHandler animationHandler, EnemyModel enemyModel, LocalSettings localSettings, SignalBus signalBus)
    {
        _enemyObservables = enemyObservables;
        _animationHandler = animationHandler;
        _enemyModel = enemyModel;
        _localSettings = localSettings;
        _signalBus = signalBus;
    }

    #endregion

    #region MaterialPropertyBlock

    private MaterialPropertyBlock _materialProperty;
    private Color _baseColor;

    #endregion


    public void Initialize()
    {
        _signalBus.Subscribe<SignalEnemySpawned>(x =>
        {
            _localSettings.BodyCollider.enabled = true;
            CreateMaterialProperty();
        });
        _signalBus.Subscribe<SignalTakeDamage>(x => OnPlayerHit(x.Collider, x.Damage));
        CreateMaterialProperty();
    }


    private void CreateMaterialProperty()
    {
        _localSettings.BodyRenderer.SetPropertyBlock(null);
        _materialProperty = new MaterialPropertyBlock();
        _baseColor = _localSettings.BodyRenderer.sharedMaterial.color;
    }

    private void OnPlayerHit(Collider collider, float attackDamage = 0)
    {
        _animationHandler.Play(AnimationClips.NONE, AnimationLayer.UPPER);
        _animationHandler.CrossFadeInFixed(AnimationClips.TAKEHIT, AnimationLayer.DEFAULT, 0.1F);
        _enemyModel.LocalStats.Health -= attackDamage;
        _signalBus.AbstractFire(new SignalEnemyTakeHit(new EnemyParticleSignalData(
            EnemyParticleHandler.ParticleType.TAKEHIT,
            collider.ClosestPoint(_enemyModel.Position))));
        if (_enemyModel.LocalStats.Health <= 0)
        {
            _localSettings.BodyCollider.enabled = false;
            var hitPosition = collider.ClosestPoint(_enemyModel.Position) + _enemyModel.GO.transform.forward;
            _signalBus.AbstractFire(new SignalEnemyDeath(hitPosition,
                new EnemyParticleSignalData(EnemyParticleHandler.ParticleType.DEATH)));
            _signalBus.AbstractFire(new SignalEnemyKilled(_enemyModel.EnemyStatsSo.ExperiencePoint,
                _enemyModel.EnemyStatsSo.SkullAmount, _enemyModel.Position, _enemyModel.LocalStats.Guid));
            _enemyObservables.SetState(EnemyState.DIED);
        }
        else
        {
            _enemyObservables.SetState(EnemyState.TAKINGDAMAGE);
            KnockBack();
            FadeBodyColor();
        }
    }

    private async Task FadeBodyColor()
    {
        var time = 0f;

        while (time < 0.5f)
        {
            time += Time.deltaTime;
            _materialProperty.SetColor("_BaseColor", Color.Lerp(Color.white, _baseColor, time / 0.5f));
            _localSettings.BodyRenderer.SetPropertyBlock(_materialProperty);
            await Task.Yield();
        }
    }

    private async Task KnockBack()
    {
        var time = 0f;

        while (time < 0.1f)
        {
            time += Time.deltaTime;
            if (_enemyModel.NavMeshAgent.enabled)
                _enemyModel.NavMeshAgent.Move(-_enemyModel.GO.transform.forward * Time.deltaTime * 2);
            await Task.Yield();
        }
    }


    public void Dispose()
    {
        _disposables?.Dispose();
    }

    [Serializable]
    public class LocalSettings
    {
        public Renderer BodyRenderer;
        public Collider BodyCollider;
    }
}