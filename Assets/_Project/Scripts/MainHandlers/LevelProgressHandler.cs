using System;
using MainHandlers;
using PlayerBehaviors;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class LevelProgressHandler : IInitializable, IDisposable
{
    #region INJECT

    private readonly Settings _settings;
    private readonly PlayerFacade _player;
    private readonly GameObservables _gameObservables;
    private readonly CompositeDisposable _compositeDisposable = new();

    private LevelProgressHandler(Settings settings, PlayerFacade player, GameObservables gameObservables)
    {
        _settings = settings;
        _player = player;
        _gameObservables = gameObservables;
    }

    #endregion

    private float _fullDistance;


    public void Initialize()
    {
        _fullDistance = CalculateDistance();
        SetLevelTexts();

        _gameObservables.GameStateUpdateObservable.Where(x => x == GameStateManager.GameStates.InGameState)
            .Subscribe(_ => CheckAndUpdateProgress()).AddTo(_compositeDisposable);
    }

    private void SetLevelTexts()
    {
        var currentLevel = SceneManager.GetActiveScene().buildIndex;
        _settings.uiStartText.text = currentLevel.ToString();
        _settings.uiEndText.text = (currentLevel + 1).ToString();
    }


    private float CalculateDistance()
    {
        return (_settings.EndLineTransform.position - _player.transform.position).sqrMagnitude;
    }

    private void CheckAndUpdateProgress()
    {
        var newDistance = CalculateDistance();
        var lerpedDistance = Mathf.InverseLerp(_fullDistance, 0f, newDistance);
        UpdateProgressFill(lerpedDistance);
    }


    private void UpdateProgressFill(float value)
    {
        _settings.uiFillImage.fillAmount = value;
    }

    public void Dispose()
    {
        _compositeDisposable?.Dispose();
    }


    [Serializable]
    public class Settings
    {
        public TextMeshProUGUI uiStartText;
        public TextMeshProUGUI uiEndText;
        public Image uiFillImage;
        public Transform EndLineTransform;
    }
}