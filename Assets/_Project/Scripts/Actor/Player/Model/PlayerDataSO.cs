using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "MonsterFarm/Data/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    public IntArraySO ExperienceRequirementsByLevel;
    public List<ItemDataSO> ItemDatas;
    public List<SkillDataSO> SkillDatas;
}