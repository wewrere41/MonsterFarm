using Events;

public class CornetSkill : DamageableSkillBase
{
    private CornetSpawner _cornetSpawner;

    public override void Initialize(SkillParticleSignalData particleData, HitType hitType)
    {
        _cornetSpawner ??= GetComponent<CornetSpawner>();
        _cornetSpawner.InitializeSpawner(particleData);
    }
}