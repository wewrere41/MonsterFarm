using System;
using Events;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class ChapterText : IInitializable
{
    #region INJECT

    private readonly UISettings _uiSettings;
    private readonly WorldStatsSO _worldStats;
    private readonly SignalBus _signalBus;

    public ChapterText(UISettings uiSettings, WorldStatsSO worldStats, SignalBus signalBus)
    {
        _uiSettings = uiSettings;
        _worldStats = worldStats;
        _signalBus = signalBus;
    }

    #endregion

    public void Initialize()
    {
        _signalBus.Subscribe<SignalLevelCompleted>(_ => UpdateChapterText());
        UpdateChapterText();
    }

    private void UpdateChapterText()
    {
        _uiSettings.ChapterText.text =
            $"CHAPTER {SceneManager.GetActiveScene().buildIndex}:{_worldStats.LevelIndex + 1}";
        _uiSettings.ChapterProgressImage.fillAmount = _worldStats.LevelIndex / (float) 5;
    }


    [Serializable]
    public class UISettings
    {
        public TextMeshProUGUI ChapterText;
        public Image ChapterProgressImage;
    }
}