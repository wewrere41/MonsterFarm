using System;
using System.Linq;
using Events;
using PlayerBehaviors.Helper;
using UnityEngine;
using Zenject;

public class PlayerSkillCooldownUIController : IInitializable
{
    #region INJECT

    private readonly UISettings _uıSettings;
    private readonly SignalBus _signalBus;

    public PlayerSkillCooldownUIController(UISettings uıSettings, SignalBus signalBus)
    {
        _uıSettings = uıSettings;
        _signalBus = signalBus;
    }

    #endregion

    private SkillCooldownUI[] _skillCooldownUIs = new SkillCooldownUI[7];
    private readonly int[] _activeSkillCooldownUIs = { -1, -1, -1, -1, -1, -1, -1 };

    public void Initialize()
    {
        _signalBus.Subscribe<SignalPlayerSkillUpgrade>((x) => CheckSkillUpgrade(x.SkillType));
        InitializeSkillCooldownUIs();
    }

    private void InitializeSkillCooldownUIs()
    {
        _skillCooldownUIs = _uıSettings.SkillParent.GetComponentsInChildren<SkillCooldownUI>();
        for (var i = 0; i < _skillCooldownUIs.Length; i++)
        {
            if (PlayerSkillHelper.GetSkillLevel((SkillTypes)i) > 0)
            {
                var skillDataSo = PlayerSkillHelper.GetSkillData((SkillTypes)i);
                var isActiveSkill = skillDataSo.IsActiveSkill;
                var skillCooldownUI = _skillCooldownUIs[i];
                var cooldown = PlayerSkillHelper.GetSkillCooldown(skillDataSo.SkillType);
                _activeSkillCooldownUIs[i] = i;
                skillCooldownUI.Initialize(skillDataSo.SkillType, skillDataSo.Icon, isActiveSkill, cooldown, false);
            }
        }
    }

    private void CheckSkillUpgrade(SkillTypes skillType)
    {
        var skillIndex = (int)skillType;

        var skillData = PlayerSkillHelper.GetSkillData(skillType);
        var isActiveSkill = skillData.IsActiveSkill;
        var skillCooldownUI = _skillCooldownUIs[skillIndex];
        var cooldown = PlayerSkillHelper.GetSkillCooldown(skillData.SkillType);

        if (!_activeSkillCooldownUIs.Contains(skillIndex))
        {
            _activeSkillCooldownUIs[skillIndex] = skillIndex;
            skillCooldownUI.Initialize(skillData.SkillType, skillData.Icon, isActiveSkill, cooldown, true);
        }
        else
        {
            skillCooldownUI.UpdateCoolDown(cooldown);
        }
    }

    [Serializable]
    public class UISettings
    {
        public Transform SkillParent;
    }
}