using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "MonsterFarm/Data/Item Data")]
public class ItemDataSO : ScriptableObject
{
    [HorizontalGroup("Info",75),HideLabel]
    [PreviewField(70)] public Sprite ItemSprite;

    [VerticalGroup("Info/Right",PaddingTop = 10),LabelWidth(70)]
    public ItemTypes ItemType;

    [VerticalGroup("Info/Right"),LabelWidth(70)]
    public string ItemName;

    [ListDrawerSettings(ShowIndexLabels = true)]
    public ItemData[] DataArray;
}

[Serializable]
public struct ItemData
{
    [LabelText("Next Level Gold")] public int NextLevelGoldRequirement;
    public float Value;
}

public enum ItemTypes
{
    WEAPON,
    BODY_ARMOR,
    HEAD_ARMOR,
    FOOT_ARMOR,
    INVENTORY
}