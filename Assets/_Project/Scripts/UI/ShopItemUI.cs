using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public ItemTypes ItemType { get; private set; }
    [field: SerializeField] public Button UpgradeButton { get; private set; }
    [field: SerializeField] private Transform ItemLevelImageParent { get; set; }
    [field: SerializeField] private Image ItemImage { get; set; }
    [field: SerializeField] private TextMeshProUGUI ItemNameText { get; set; }
    [field: SerializeField] private TextMeshProUGUI ItemLevelText { get; set; }
    [field: SerializeField] private TextMeshProUGUI GoldRequirementText { get; set; }


    private int _itemMaxLevel;

    public void Initialize(ItemTypes itemType, Sprite icon, string itemName, int itemLevel, int itemMaxLevel,
        string goldRequirement)
    {
        ItemType = itemType;
        ItemImage.sprite = icon;
        ItemNameText.text = itemName;
        _itemMaxLevel = itemMaxLevel;
        ItemLevelText.text = $"{itemLevel}/{_itemMaxLevel}";
        GoldRequirementText.text = goldRequirement;

        var levelImageCount = itemLevel > 5 ? itemLevel % 5 : itemLevel;
        for (var i = 0; i < levelImageCount; i++)
        {
            ItemLevelImageParent.GetChild(i).GetComponent<Image>().color = Color.yellow;
        }
    }

    public void UpdateUI(int itemLevel, string goldRequirement)
    {
        ItemLevelText.text = $"{itemLevel}/{_itemMaxLevel}";
        GoldRequirementText.text = goldRequirement;
        var levelImageCount = itemLevel > 5 ? itemLevel % 5 : itemLevel;
        for (var i = 0; i < levelImageCount; i++)
        {
            ItemLevelImageParent.GetChild(i).GetComponent<Image>().color = Color.yellow;
        }
    }
}