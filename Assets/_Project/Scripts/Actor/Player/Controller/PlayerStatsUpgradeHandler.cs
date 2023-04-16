using Events;
using PlayerBehaviors;
using PlayerBehaviors.Helper;
using Zenject;

public class PlayerStatsUpgradeHandler : IInitializable
{
    #region INJECT

    private readonly PlayerModel _playerModel;
    private readonly SignalBus _signalBus;

    public PlayerStatsUpgradeHandler(PlayerModel playerModel, SignalBus signalBus)
    {
        _playerModel = playerModel;
        _signalBus = signalBus;
    }

    #endregion

    public void Initialize()
    {
        _signalBus.Subscribe<SignalPlayerItemUpgrade>(x => UpdatePlayerStat(x.ItemType));
        _signalBus.Subscribe<SignalPlayerSkillUpgrade>(x => UpdatePlayerStat(x.SkillType));

        InitAllPlayerStats();
        
    }

    private void InitAllPlayerStats()
    {
        UpdatePlayerStat(ItemTypes.WEAPON);
        UpdatePlayerStat(ItemTypes.BODY_ARMOR);
        UpdatePlayerStat(SkillTypes.PASSIVE_AttackSpeed);
        UpdatePlayerStat(SkillTypes.PASSIVE_MovementSpeed);
        _playerModel.UpdateLocalHealth();
    }

    private void UpdatePlayerStat(ItemTypes itemType)
    {
        switch (itemType)
        {
            case ItemTypes.WEAPON:
                var itemValue = PlayerItemHelper.GetItemValue(itemType);
                PlayerStatHelper.SetStatValue(itemType,
                    itemValue + itemValue / 100 * PlayerSkillHelper.GetSkillValue(SkillTypes.PASSIVE_AttackDamage));
                break;
            case ItemTypes.BODY_ARMOR:
            case ItemTypes.HEAD_ARMOR:
            case ItemTypes.FOOT_ARMOR:

                var bodyArmor = PlayerItemHelper.GetItemValue(ItemTypes.BODY_ARMOR);
                var headArmor = PlayerItemHelper.GetItemValue(ItemTypes.HEAD_ARMOR);
                var footArmor = PlayerItemHelper.GetItemValue(ItemTypes.FOOT_ARMOR);
                var totalArmor = bodyArmor + headArmor + footArmor;

                PlayerStatHelper.SetStatValue(itemType, totalArmor +
                                                        totalArmor / 100 *
                                                        PlayerSkillHelper.GetSkillValue(SkillTypes
                                                            .PASSIVE_Health));
                _playerModel.UpdateLocalHealth();
                break;
        }
    }

    private void UpdatePlayerStat(SkillTypes skillType)
    {
        switch (skillType)
        {
            case SkillTypes.PASSIVE_AttackDamage:
                var itemValue = PlayerItemHelper.GetItemValue(skillType.ToItemType());
                PlayerStatHelper.SetStatValue(skillType,
                    itemValue + itemValue / 100 * PlayerSkillHelper.GetSkillValue(skillType));
                break;
            case SkillTypes.PASSIVE_Health:
                var bodyArmor = PlayerItemHelper.GetItemValue(ItemTypes.BODY_ARMOR);
                var headArmor = PlayerItemHelper.GetItemValue(ItemTypes.HEAD_ARMOR);
                var footArmor = PlayerItemHelper.GetItemValue(ItemTypes.FOOT_ARMOR);
                var totalArmor = bodyArmor + headArmor + footArmor;

                PlayerStatHelper.SetStatValue(skillType, totalArmor +
                                                         totalArmor / 100 *
                                                         PlayerSkillHelper.GetSkillValue(SkillTypes
                                                             .PASSIVE_Health));
                _playerModel.UpdateLocalHealth();
                break;
            case SkillTypes.PASSIVE_AttackSpeed:
            case SkillTypes.PASSIVE_MovementSpeed:
                PlayerStatHelper.SetStatValue(skillType, PlayerSkillHelper.GetSkillValue(skillType));
                PlayerStatHelper.SetStatValue(skillType, PlayerSkillHelper.GetSkillValue(skillType));
                break;
        }
    }
}