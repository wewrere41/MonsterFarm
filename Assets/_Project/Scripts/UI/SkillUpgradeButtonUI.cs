using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpgradeButtonUI : MonoBehaviour
{
    [field: SerializeField] public Image SkillImage { get; private set; }
    [field: SerializeField] public Button UpgradeButton { get; private set; }
    [field: SerializeField] public TextMeshProUGUI StatLevelText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SkillNameText { get; private set; }


    public void UpdateUI(Sprite skillIcon, string skillName, string levelText)
    {
        SkillImage.sprite = skillIcon;
        SkillNameText.text = skillName;
        StatLevelText.text = levelText;
    }
}