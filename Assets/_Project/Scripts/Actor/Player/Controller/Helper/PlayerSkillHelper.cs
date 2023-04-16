using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace PlayerBehaviors.Helper
{
    public class PlayerSkillHelper
    {
        #region INJECT

        private static PlayerFacade _playerFacade;

        [Inject]
        public void Construct(PlayerFacade playerFacade, PlayerDataSO playerDataSo)
        {
            _playerFacade = playerFacade;
            _playerSkillDataDictionary = playerDataSo.SkillDatas.ToDictionary(x => x.SkillType);
        }

        #endregion

        public static SkillDataSO GetSkillData(SkillTypes skillTypes) => _playerSkillDataDictionary[skillTypes];

        public static ref int GetSkillLevel(SkillTypes skillTypes)
        {
            switch (skillTypes)
            {
                case SkillTypes.PASSIVE_AttackDamage:
                    return ref _playerFacade.PlayerStatsSo.Passive_DamageBonus_Level;
                case SkillTypes.PASSIVE_Health:
                    return ref _playerFacade.PlayerStatsSo.Passive_HealthBonus_Level;
                case SkillTypes.PASSIVE_AttackSpeed:
                    return ref _playerFacade.PlayerStatsSo.Passive_AttackSpeed_Level;
                case SkillTypes.PASSIVE_MovementSpeed:
                    return ref _playerFacade.PlayerStatsSo.Passive_MovementSpeed_Level;
                case SkillTypes.ACTIVE_0:
                    return ref _playerFacade.PlayerStatsSo.Active_Skill0_Level;
                case SkillTypes.ACTIVE_1:
                    return ref _playerFacade.PlayerStatsSo.Active_Skill1_Level;
                case SkillTypes.ACTIVE_2:
                    return ref _playerFacade.PlayerStatsSo.Active_Skill2_Level;
                default:
                    throw new ArgumentOutOfRangeException(nameof(skillTypes), skillTypes, null);
            }
        }

        public static void IncreaseSkillLevel(SkillTypes skillType)
        {
            if (CheckSkillCap(skillType) is false) GetSkillLevel(skillType)++;
        }

        public static float GetSkillValue(SkillTypes skillType) =>
            _playerSkillDataDictionary[skillType].SkillDataArray[GetSkillLevel(skillType)].Value;

        public static float GetSkillCooldown(SkillTypes skillType) =>
            _playerSkillDataDictionary[skillType].SkillDataArray[GetSkillLevel(skillType)].Cooldown;

        public static float GetSkillDuration(SkillTypes skillType) =>
            _playerSkillDataDictionary[skillType].SkillDataArray[GetSkillLevel(skillType)].Duration;

        public static int SkillCount => _playerSkillDataDictionary.Count;

        private static Dictionary<SkillTypes, SkillDataSO> _playerSkillDataDictionary;

        private static bool CheckSkillCap(SkillTypes skillType) =>
            GetSkillLevel(skillType) == _playerSkillDataDictionary[skillType].SkillDataArray.Length - 1;
    }

    public static class SkillHelperExtensions
    {
        public static ItemTypes ToItemType(this SkillTypes skillTypes)
        {
            return skillTypes switch
            {
                SkillTypes.PASSIVE_AttackDamage => ItemTypes.WEAPON,
                SkillTypes.PASSIVE_Health => ItemTypes.BODY_ARMOR,
                _ => throw new ArgumentOutOfRangeException(nameof(skillTypes), skillTypes, null)
            };
        }
    }
}