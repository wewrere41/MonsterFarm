using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Events;
using PlayerBehaviors;
using PlayerBehaviors.Helper;
using UnityEngine;
using Utilities;
using Zenject;
using Random = UnityEngine.Random;

public class PlayerSkillUpgradeController : IInitializable, IDisposable
{
    #region INJECT

    private readonly PlayerModel _playerModel;
    private readonly SignalBus _signalBus;
    private readonly UISettings _uiSettings;

    public PlayerSkillUpgradeController(PlayerModel playerModel, SignalBus signalBus, UISettings uiSettings)
    {
        _playerModel = playerModel;
        _signalBus = signalBus;
        _uiSettings = uiSettings;
    }

    #endregion

    private readonly CancellationTokenSource _ctx = new();
    private SkillUpgradeButtonUI[] _upgradeButtons;
    private readonly int[] _randomSkillIndexes = new int[3];

    public void Initialize()
    {
        _signalBus.Subscribe<SignalPlayerLevelUp>(x => OpenSkillUpgradePanel());
        InitializeUpgradeButtons();
    }


    private void InitializeUpgradeButtons()
    {
        _upgradeButtons = new SkillUpgradeButtonUI[3];
        for (var i = 0; i < 3; i++)
            _upgradeButtons[i] = _uiSettings.SkillLayoutPanel.GetChild(i).GetComponent<SkillUpgradeButtonUI>();
    }

    private void ShuffleSkills()
    {
        for (var i = 0; i < _randomSkillIndexes.Length; i++)
            _randomSkillIndexes[i] = -1;

        for (var i = 0; i < _upgradeButtons.Length; i++)
        {
            var skillUpgradeButton = _upgradeButtons[i];

            var randomIndex = Random.Range(0, PlayerSkillHelper.SkillCount);
            while (_randomSkillIndexes.Contains(randomIndex))
                randomIndex = Random.Range(0, PlayerSkillHelper.SkillCount);

            _randomSkillIndexes[i] = randomIndex;

            var skillType = (SkillTypes)randomIndex;

            UpdateSkillUpgradeUI(i, skillType);
            skillUpgradeButton.UpgradeButton.onClick.AddListener(() =>
            {
                CloseStatsUpgradePanel();
                UpgradeSkillLevel(skillType);
                _signalBus.AbstractFire(new SignalPlayerSkillUpgrade(skillType,
                    new BaseParticleSignalData(PlayerParticleHandler.ParticleType.SKILL_UPGRADE)));
                _signalBus.Fire(new SignalJoystickSetActive(true));
            });
        }
    }

    private void UpdateSkillUpgradeUI(int buttonIndex, SkillTypes skillType)
    {
        var skillButtonData = _upgradeButtons[buttonIndex];
        var skillData = PlayerSkillHelper.GetSkillData(skillType);
        var icon = skillData.Icon;
        var skillLevel = "LV" + (PlayerSkillHelper.GetSkillLevel(skillType) + 1);
        var skillName = skillData.SkillName;
        skillButtonData.UpdateUI(icon, skillName, skillLevel);
    }

    private void UpgradeSkillLevel(SkillTypes skillType)
    {
        PlayerSkillHelper.IncreaseSkillLevel(skillType);
    }


    #region PANEL UI

    private async Task OpenSkillUpgradePanel()
    {
        ShuffleSkills();

        await Task.Delay(500, _ctx.Token);

        _signalBus.Fire(new SignalJoystickSetActive(false));

        _uiSettings.LevelUpPanel.gameObject.SetActive(true);

        Time.timeScale = 0;
    }

    private void CloseStatsUpgradePanel()
    {
        Time.timeScale = 1;
        _uiSettings.LevelUpPanel.gameObject.SetActive(false);
        foreach (var skillButtonData in _upgradeButtons)
        {
            skillButtonData.UpgradeButton.onClick.RemoveAllListeners();
        }
    }

    #endregion


    [Serializable]
    public class UISettings
    {
        public RectTransform LevelUpPanel;
        public RectTransform SkillLayoutPanel;
    }


    public void Dispose()
    {
        _ctx.Cancel();
        _ctx.Dispose();
    }
}