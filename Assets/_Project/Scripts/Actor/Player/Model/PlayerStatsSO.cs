using Constants;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "MonsterFarm/Stats/Player")]
public class PlayerStatsSO : BaseStatsSO
{
    [BoxGroup("Player Specific", CenterLabel = true), LabelText("Player Level")]
    public int Level;

    [BoxGroup("Player Specific", CenterLabel = true)]
    public int TotalXp;

    [BoxGroup("Player Specific", CenterLabel = true)]
    public int SkullCount;

    [BoxGroup("Player Specific", CenterLabel = true)]
    public int GoldCount;

    [BoxGroup("Player Specific", CenterLabel = true)]
    public Vector3 LastPosition;

    [Title("ACTIVE SKILLS", titleAlignment: TitleAlignments.Centered)] [SerializeField]
    public int Active_Skill0_Level;

    [SerializeField] public int Active_Skill1_Level;
    [SerializeField] public int Active_Skill2_Level;

    [Title("PASSIVE SKILLS", titleAlignment: TitleAlignments.Centered)] [LabelText("AD Bonus Level")]
    public int Passive_DamageBonus_Level;

    [LabelText("HP Bonus Level")] public int Passive_HealthBonus_Level;

    [LabelText("Attack Speed Level")]
    public int Passive_AttackSpeed_Level;

    [LabelText("Movement Speed Level")]
    public int Passive_MovementSpeed_Level;

    [Title("ITEMS", titleAlignment: TitleAlignments.Centered)] [LabelText("Weapon Level")]
    public int Item_Weapon_Level;

    [LabelText("Body Armor Level")] public int Item_BodyArmor_Level;
    [LabelText("Head Armor Level")] public int Item_HeadArmor_Level;
    [LabelText("Foot Armor Level")] public int Item_FootArmor_Level;
    [LabelText("Inventory Level")] public int Item_Inventory_Level;


    [JsonIgnore] public bool LoadFromJson;
    [JsonIgnore] public bool LoadPosition;

    [Button]
    public void Save()
    {
        JsonOP.SerializeData(this, DataPaths.PLAYER_STATS);
    }

    [Button]
    public override void Reset()
    {
        AttackDamage = 25;
        Health = 100;
        AttackSpeed = 1.5f;
        MovementSpeed = 8;

        Level = 1;
        TotalXp = 0;
        SkullCount = 0;
        GoldCount = 0;
        LastPosition = Vector3.zero;

        Passive_DamageBonus_Level = 0;
        Passive_HealthBonus_Level = 0;
        Passive_AttackSpeed_Level = 0;
        Passive_MovementSpeed_Level = 0;

        Active_Skill0_Level = 0;
        Active_Skill1_Level = 0;
        Active_Skill2_Level = 0;

        Item_Weapon_Level = 1;
        Item_BodyArmor_Level = 0;
        Item_HeadArmor_Level = 0;
        Item_FootArmor_Level = 0;
        Item_Inventory_Level = 1;

        JsonOP.RemoveData(DataPaths.PLAYER_STATS);
    }
}