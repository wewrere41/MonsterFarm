using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

[CreateAssetMenu(fileName = "SkillData", menuName = "MonsterFarm/Data/Skill Data")]
public class SkillDataSO : ScriptableObject
{
    [HorizontalGroup("Info", 75), HideLabel] [VerticalGroup("Info/Left", PaddingTop = 20)] [PreviewField(70)]
    public Sprite Icon;

    [VerticalGroup("Info/Right"), LabelWidth(120)]
    public bool IsActiveSkill;

    [VerticalGroup("Info/Right", PaddingTop = 10), LabelWidth(120)]
    public string SkillName;

    [VerticalGroup("Info/Right"), LabelWidth(120)]
    public SkillTypes SkillType;

    [VerticalGroup("Info/Right"), LabelWidth(120)]
    public string AnimationName;

    [VerticalGroup("Info/Right"), LabelWidth(120)]
    public PlayerParticleHandler.ParticleType ParticleEffect;

    [ListDrawerSettings(ShowIndexLabels = true)]
    public SkillData[] SkillDataArray;
}

[Serializable]
public struct SkillData
{
    public float Value;
    public float Duration;
    public float Cooldown;
}

public enum SkillTypes
{
    ACTIVE_0 = 0,
    ACTIVE_1 = 1,
    ACTIVE_2 = 2,
    PASSIVE_AttackDamage = 3,
    PASSIVE_Health = 4,
    PASSIVE_AttackSpeed = 5,
    PASSIVE_MovementSpeed = 6,
}