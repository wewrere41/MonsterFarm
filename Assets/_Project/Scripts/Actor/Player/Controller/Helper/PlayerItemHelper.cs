using System;
using System.Collections.Generic;
using System.Linq;
using PlayerBehaviors;
using Zenject;

public class PlayerItemHelper
{
    #region INJECT

    private static PlayerFacade _playerFacade;
    private static PlayerDataSO _playerDataSO;

    [Inject]
    public void Construct(PlayerFacade playerFacade, PlayerDataSO playerDataSo)
    {
        _playerFacade = playerFacade;
        _playerDataSO = playerDataSo;

        _itemDataSODict = _playerDataSO.ItemDatas.ToDictionary(x => x.ItemType);
    }

    #endregion

    public static ItemDataSO GetItemData(ItemTypes itemType) => _itemDataSODict[itemType];

    public static ref int GetItemLevel(ItemTypes itemType)
    {
        switch (itemType)
        {
            case ItemTypes.WEAPON:
                return ref _playerFacade.PlayerStatsSo.Item_Weapon_Level;
            case ItemTypes.BODY_ARMOR:
                return ref _playerFacade.PlayerStatsSo.Item_BodyArmor_Level;
            case ItemTypes.HEAD_ARMOR:
                return ref _playerFacade.PlayerStatsSo.Item_HeadArmor_Level; 
            case ItemTypes.FOOT_ARMOR:
                return ref _playerFacade.PlayerStatsSo.Item_FootArmor_Level;
            case ItemTypes.INVENTORY:
                return ref _playerFacade.PlayerStatsSo.Item_Inventory_Level;

            default:
                throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
        }
    }

    public static void IncreaseItemLevel(ItemTypes itemType) => GetItemLevel(itemType)++;


    public static float GetItemValue(ItemTypes itemType) =>
        _itemDataSODict[itemType].DataArray[GetItemLevel(itemType)].Value;

    public static int NextLevelGoldCost(ItemTypes itemType) =>
        _itemDataSODict[itemType].DataArray[GetItemLevel(itemType)].NextLevelGoldRequirement;

    public static bool CheckItemCap(ItemTypes itemType) =>
        GetItemLevel(itemType) == _itemDataSODict[itemType].DataArray.Length - 1;


    private static Dictionary<ItemTypes, ItemDataSO> _itemDataSODict = new();
}