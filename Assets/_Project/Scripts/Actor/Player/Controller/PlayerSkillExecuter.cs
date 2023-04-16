using System;
using Constants;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Events;
using MainHandlers;
using PlayerBehaviors;
using PlayerBehaviors.Helper;
using UniRx;
using UnityEngine;
using Utilities;
using Zenject;

public class PlayerSkillExecuter : IInitializable, IDisposable
{
    #region INJECT

    private readonly PlayerModel _playerModel;
    private readonly SignalBus _signalBus;
    private readonly PlayerObservables _playerObservables;
    private readonly GameObservables _gameObservables;
    private readonly AnimationHandler _animationHandler;

    public PlayerSkillExecuter(PlayerModel playerModel, SignalBus signalBus, PlayerObservables playerObservables,
        AnimationHandler animationHandler, GameObservables gameObservables)
    {
        _playerModel = playerModel;
        _signalBus = signalBus;
        _playerObservables = playerObservables;
        _animationHandler = animationHandler;
        _gameObservables = gameObservables;
    }

    #endregion

    private readonly CompositeDisposable _compositeDisposable = new();
    private TweenerCore<Quaternion, Vector3, QuaternionOptions> _rotateTween;
    private IDisposable _skillCompleteTimer;

    public void Initialize()
    {
        _signalBus.Subscribe<SignalPlayerPlaySkill>(x => PlaySkill(x.Skilltype));
        _signalBus.Subscribe<SignalStopSkill>(StopSkill);
    }

    private async void PlaySkill(SkillTypes skillType)
    {
        var skillData = PlayerSkillHelper.GetSkillData(skillType);
        if (skillData.IsActiveSkill is false)
            return;

        var damage = PlayerSkillHelper.GetSkillValue(skillType);
        var skillDuration = PlayerSkillHelper.GetSkillDuration(skillType);
        var animation = skillData.AnimationName;
        var particleType = skillData.ParticleEffect;

        PlayAnimation(animation);

        await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));

        var animationInterval = _animationHandler.GetAnimationInterval(AnimationLayer.DEFAULT);
        var timerDuration = skillDuration == 0 ? animationInterval : skillDuration;
        var delay = skillDuration == 0 ? animationInterval / 2 : 0;

        PlayParticle(particleType, damage, delay, skillDuration);
        var durationObservable = CreateSkillTimer(timerDuration);
        CreateDefaultObserver(skillType, durationObservable);

        switch (skillType)
        {
            case SkillTypes.ACTIVE_1:
                _playerObservables.SetPlayerState(PlayerState.SKILL);
                _signalBus.Fire(new SignalSetActiveCollider(true));
                _rotateTween = _playerModel.GO.transform
                    .DORotate(new Vector3(0, 1440 * skillDuration, 0), skillDuration, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.Linear);
                durationObservable.Subscribe(x => ResetPlayerState()).AddTo(_compositeDisposable);
                break;
            case SkillTypes.ACTIVE_2:
                _playerObservables.SetPlayerState(PlayerState.SKILL);
                durationObservable.Subscribe(x => ResetPlayerState()).AddTo(_playerModel.GO).AddTo(_compositeDisposable);
                break;
        }
    }

    private void StopSkill()
    {
        ResetPlayerState();
        _gameObservables.SetSkillCanPlayable(false);
        _skillCompleteTimer?.Dispose();
        _rotateTween.Kill();
    }

    #region BASE EXECUTIONS

    private void PlayParticle(PlayerParticleHandler.ParticleType particleType, float damage, float delay,
        float duration)
    {
        var detach = particleType == PlayerParticleHandler.ParticleType.SKILL_2;
        if (particleType != PlayerParticleHandler.ParticleType.NONE)
            Observable.Timer(TimeSpan.FromSeconds(delay))
                .Subscribe(x =>
                    _signalBus.AbstractFire(
                        new SignalPlaySkillParticle(new(particleType, damage, detach,
                            duration: duration)))).AddTo(_compositeDisposable);
    }

    private void PlayAnimation(string animation)
    {
        if (string.IsNullOrEmpty(animation) is false)
        {
            _animationHandler.Play(Animator.StringToHash(animation), AnimationLayer.DEFAULT);
            _animationHandler.Play(AnimationClips.NONE, AnimationLayer.UPPER);
        }
    }

    private static IObservable<long> CreateSkillTimer(float duration)
    {
        var durationTimer = Observable.Timer(TimeSpan.FromSeconds(duration));
        return durationTimer;
    }

    private void CreateDefaultObserver(SkillTypes skillType, IObservable<long> timerObservable)
    {
        _skillCompleteTimer = timerObservable.Subscribe(x => _signalBus.Fire(new SignalPlayerSkillCompleted(skillType)))
            .AddTo(_compositeDisposable);
    }

    #endregion

    private void ResetPlayerState()
    {
        _playerObservables.SetPlayerState(PlayerState.WALK);
        _animationHandler.CrossFadeInFixed(AnimationClips.WALK, AnimationLayer.DEFAULT, 0.25f);
        _signalBus.Fire(new SignalSetActiveCollider(false));
    }

    public void Dispose()
    {
        _compositeDisposable?.Dispose();
    }
}