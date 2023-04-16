using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Events;
using MainHandlers;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class PlayerSkillRegistry : IInitializable,IDisposable
{
    #region INJECT

    private readonly SignalBus _signalBus;
    private readonly GameObservables _gameObservables;

    public PlayerSkillRegistry(SignalBus signalBus, GameObservables gameObservables)
    {
        _signalBus = signalBus;
        _gameObservables = gameObservables;
    }

    #endregion

    private readonly CompositeDisposable _disposables = new();
    private CancellationTokenSource _cts = new();
    private readonly Queue<SkillTypes> _skillQueue = new(3);
    private float _lastSkillTime;

    private SkillExecutionState _skillExecutionState = SkillExecutionState.NONE;

    private enum SkillExecutionState
    {
        NONE,
        WAITING,
        WILLPLAY,
        PLAYING
    }

    public void Initialize()
    {
        _signalBus.Subscribe<SignalPlayerSkillReady>(x => EnqueueSkill(x.Skilltype));
        _signalBus.Subscribe<SignalPlayerSkillCompleted>(_ => DequeueSkill());
        _signalBus.Subscribe<SignalStopSkill>(StopActiveSkill);

        _skillExecutionState = SkillExecutionState.WAITING;

        WhenCanPlaySkillAndHasWaitingSkill()
            .Subscribe(x => PlayNextSkill()).AddTo(_disposables);
    }


    private void EnqueueSkill(SkillTypes skillType)
    {
        if (_skillQueue.Contains(skillType)) return;

        _skillQueue.Enqueue(skillType);

        if (_skillExecutionState == SkillExecutionState.WAITING)
        {
            PlayNextSkill();
        }
    }

    private void DequeueSkill()
    {
        if (_skillQueue.Count <= 0) return;

        _lastSkillTime = Time.time;
        _skillExecutionState = SkillExecutionState.WAITING;
        _skillQueue.Dequeue();

        if (_skillQueue.Count > 0)
            PlayNextSkill();
    }

    private void StopActiveSkill()
    {
        if (_skillExecutionState == SkillExecutionState.PLAYING)
        {
            _skillExecutionState = SkillExecutionState.WAITING;
            _skillQueue.Dequeue();
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
        }
    }


    private async Task PlayNextSkill()
    {
        _skillExecutionState = SkillExecutionState.WILLPLAY;
        await UniTask.WaitUntil(() => _gameObservables.SkillCanPlay, cancellationToken: _cts.Token);
        var skillElapsedTime = Time.time - _lastSkillTime;
        var delay = skillElapsedTime < 5f ? (int)Random.Range(3000, (5 - skillElapsedTime) * 1000) : 0;

        await UniTask.Delay(delay, cancellationToken: _cts.Token);
        await UniTask.WaitUntil(() => _gameObservables.SkillCanPlay, cancellationToken: _cts.Token);
        var skillType = _skillQueue.Peek();

        _skillExecutionState = SkillExecutionState.PLAYING;
        _signalBus.Fire(new SignalPlayerPlaySkill(skillType));
    }

    #region HELPER

    private IObservable<bool> WhenCanPlaySkillAndHasWaitingSkill()
    {
        return _gameObservables.CanPlaySkillObservable().Where(x =>
            x && _skillQueue.Count > 0 && _skillExecutionState == SkillExecutionState.WAITING);
    }

    #endregion

    public void Dispose()
    {
        _disposables?.Dispose();
        _cts?.Dispose();
    }
}