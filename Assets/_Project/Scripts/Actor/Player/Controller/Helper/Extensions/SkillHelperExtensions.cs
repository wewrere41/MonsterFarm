using PlayerBehaviors.Helper;

public static class SkillHelperExtensions
{
    public static float GetSkillValue(this SkillTypes skillType) =>
        PlayerSkillHelper.GetSkillValue(skillType);

    public static float GetSkillCooldown(this SkillTypes skillType) =>
        PlayerSkillHelper.GetSkillCooldown(skillType);

    public static float GetSkillDuration(this SkillTypes skillType) =>
        PlayerSkillHelper.GetSkillDuration(skillType);


    public static float GetSkillValue(this SkillDataSO skillDataSo) =>
        PlayerSkillHelper.GetSkillValue(skillDataSo.SkillType);

    public static float GetSkillCooldown(this SkillDataSO skillDataSo) =>
        PlayerSkillHelper.GetSkillCooldown(skillDataSo.SkillType);

    public static float GetSkillDuration(this SkillDataSO skillDataSo) =>
        PlayerSkillHelper.GetSkillDuration(skillDataSo.SkillType);
}