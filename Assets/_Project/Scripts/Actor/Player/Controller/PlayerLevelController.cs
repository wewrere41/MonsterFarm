using System;
using Events;
using PlayerBehaviors;
using TMPro;
using UnityEngine.UI;
using Utilities;
using Zenject;

public class PlayerLevelController : IInitializable
{
    #region INJECT

    private readonly PlayerModel _playerModel;
    private readonly UISettings _uıSettings;
    private readonly PlayerDataSO _playerData;
    private readonly SignalBus _signalBus;

    public PlayerLevelController(PlayerModel playerModel, UISettings uıSettings, PlayerDataSO playerData,
        SignalBus signalBus)
    {
        _playerModel = playerModel;
        _uıSettings = uıSettings;
        _playerData = playerData;
        _signalBus = signalBus;
    }

    #endregion

    public void Initialize()
    {
        _signalBus.Subscribe<ISignalGainExperience>(x => UpdateXpBar(x.ExperienceCount));
        UpdateXpBar();
    }

    private void UpdateXpBar(int xpCount = 0)
    {
        var experienceRequirement = _playerData.ExperienceRequirementsByLevel[
            _playerModel.PlayerStatsSo.Level];

        _playerModel.PlayerStatsSo.TotalXp += xpCount;
        if (_playerModel.PlayerStatsSo.TotalXp >= experienceRequirement)
        {
            _playerModel.PlayerStatsSo.Level++;
            _playerModel.PlayerStatsSo.TotalXp -= experienceRequirement;

            _signalBus.AbstractFire(
                new SignalPlayerLevelUp(new BaseParticleSignalData(PlayerParticleHandler.ParticleType.LEVELUP)));
        }

        var xpRatio = (float)_playerModel.PlayerStatsSo.TotalXp /
                      experienceRequirement;
        _uıSettings.XpBarImage.fillAmount = xpRatio;
        _uıSettings.LevelText.text = $"LEVEL {_playerModel.PlayerStatsSo.Level}";
    }

    [Serializable]
    public class UISettings
    {
        public TextMeshProUGUI LevelText;
        public Image XpBarImage;
    }
}