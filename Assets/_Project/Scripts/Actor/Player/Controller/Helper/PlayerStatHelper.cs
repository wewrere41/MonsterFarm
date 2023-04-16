using System;
using PlayerBehaviors;
using Zenject;

public class PlayerStatHelper
{
    #region INJECT

    private static PlayerFacade _playerFacade;

    [Inject]
    public void Construct(PlayerFacade playerFacade, PlayerDataSO playerDataSo)
    {
        _playerFacade = playerFacade;
    }

    #endregion

    public static void SetStatValue(ItemTypes itemType, float value)
    {
        switch (itemType)
        {
            case ItemTypes.WEAPON:
                _playerFacade.PlayerStatsSo.AttackDamage = value;
                break;
            case ItemTypes.BODY_ARMOR:
            case ItemTypes.FOOT_ARMOR:
            case ItemTypes.HEAD_ARMOR:
                _playerFacade.PlayerStatsSo.Health = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
        }
    }

    public static void SetStatValue(SkillTypes skillTypes, float value)
    {
        switch (skillTypes)
        {
            case SkillTypes.PASSIVE_AttackDamage:
                _playerFacade.PlayerStatsSo.AttackDamage = value;
                break;
            case SkillTypes.PASSIVE_Health:
                _playerFacade.PlayerStatsSo.Health = value;
                break;
            case SkillTypes.PASSIVE_AttackSpeed:
                _playerFacade.PlayerStatsSo.AttackSpeed = value;
                break;
            case SkillTypes.PASSIVE_MovementSpeed:
                _playerFacade.PlayerStatsSo.MovementSpeed = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(skillTypes), skillTypes, null);
        }
    }
}