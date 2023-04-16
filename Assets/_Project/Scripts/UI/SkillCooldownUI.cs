using System.Collections;
using Events;
using MainHandlers;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SkillCooldownUI : MonoBehaviour
{
    #region INJECT

    private SignalBus _signalBus;
    private GameObservables _gameObservables;

    [Inject]
    private void Inject(SignalBus signalBus, GameObservables gameObservables)
    {
        _signalBus = signalBus;
        _gameObservables = gameObservables;
    }

    #endregion

    [SerializeField] private Image _cooldownImage;
    [SerializeField] private Image _skillImage;

    private SkillCooldownState _skillCooldownState = SkillCooldownState.NONE;
    private float _cooldown;
    private SkillTypes? _skillType;
    private WaitUntil _resumeTimer;

    private enum SkillCooldownState
    {
        NONE,
        IN_COOLDOWN,
        PLAYING
    }

    private void Awake()
    {
        _signalBus.Subscribe<SignalPlayerPlaySkill>(x =>
        {
            if (!isActiveAndEnabled || !IsActiveTypeSkill() || !IsCorrectSkill(x.Skilltype) ||
                _skillCooldownState != SkillCooldownState.IN_COOLDOWN) return;
            _skillCooldownState = SkillCooldownState.PLAYING;
        });
        _signalBus.Subscribe<SignalPlayerSkillCompleted>(x =>
        {
            if (isActiveAndEnabled && IsCorrectSkill(x.Skilltype))
                StartCoroutine(StartTimer(_cooldown));
        });
        _signalBus.Subscribe<SignalStopSkill>(_ =>
        {
            if (isActiveAndEnabled && IsActiveTypeSkill() && _skillCooldownState != SkillCooldownState.IN_COOLDOWN)
                StartCoroutine(StartTimer(_cooldown));
        });

        _resumeTimer = new WaitUntil(() => _gameObservables.SkillCanPlay);

        StartTimerWhenActive();
    }

    private void StartTimerWhenActive()
    {
        this.ObserveEveryValueChanged(x => isActiveAndEnabled)
            .Where(x => x && IsActiveTypeSkill())
            .Subscribe(x => StartCoroutine(StartTimer(_cooldown)));
    }


    public void Initialize(SkillTypes skillType, Sprite skillImage, bool isActiveSkill, float cooldown,
        bool startCooldownTimer)
    {
        _skillType = skillType;
        _cooldownImage.sprite = skillImage;
        _skillImage.sprite = skillImage;
        _cooldownImage.fillAmount = isActiveSkill ? 1 : 0;
        _cooldown = cooldown;
        if (isActiveSkill && startCooldownTimer)
        {
            StartCoroutine(StartTimer(cooldown));
        }
    }

    public void UpdateCoolDown(float cooldown)
    {
        _cooldown = cooldown;
    }

    private IEnumerator StartTimer(float time)
    {
        _skillCooldownState = SkillCooldownState.IN_COOLDOWN;
        _cooldownImage.fillAmount = 1;
        while (time > 0)
        {
            yield return _resumeTimer;
            time -= Time.deltaTime;
            _cooldownImage.fillAmount = time / _cooldown;
            yield return null;
        }

        _signalBus.Fire(new SignalPlayerSkillReady(_skillType.Value));
    }

    #region HELPER

    private bool IsActiveTypeSkill() => _cooldown != 0;

    private bool IsCorrectSkill(SkillTypes skillType) => skillType == _skillType;

    #endregion
}