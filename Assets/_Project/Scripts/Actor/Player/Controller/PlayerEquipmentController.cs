using System;
using Events;
using PlayerBehaviors;
using UnityEngine;
using Zenject;

public class PlayerEquipmentController : IInitializable
{
    #region INJECT

    private readonly LocalSettings _localSettings;
    private readonly PlayerModel _playerModel;
    private readonly SignalBus _signalBus;

    public PlayerEquipmentController(LocalSettings localSettings,
        PlayerModel playerModel, SignalBus signalBus)
    {
        _localSettings = localSettings;
        _playerModel = playerModel;
        _signalBus = signalBus;
    }

    #endregion

    private readonly GameObject[] _activeItems = new GameObject[4];


    public void Initialize()
    {
        for (int i = 0; i < _localSettings.ItemParents.Length; i++)
        {
            EquipItem((ItemTypes)i);
        }

        _signalBus.Subscribe<SignalPlayerItemUpgrade>(x => EquipItem(x.ItemType));
    }

    private void EquipItem(ItemTypes itemType)
    {
        var itemIndex = (int)itemType;
        if (itemIndex >= _localSettings.ItemParents.Length) return;
        
        if (_activeItems[itemIndex] != null)
        {
            _activeItems[itemIndex].SetActive(false);
            _activeItems[itemIndex] = null;
        }

        _activeItems[itemIndex] = _localSettings.ItemParents[itemIndex].transform
            .GetChild(PlayerItemHelper.GetItemLevel(itemType)).gameObject;
        _activeItems[itemIndex].SetActive(true);
    }


    [Serializable]
    public class LocalSettings
    {
        public Transform[] ItemParents;
    }
}