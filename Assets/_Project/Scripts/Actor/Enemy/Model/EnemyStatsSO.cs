using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "MonsterFarm/Stats/Enemy")]
public class EnemyStatsSO : BaseStatsSO
{
    [BoxGroup("Enemy Specific", CenterLabel = true)]
    public int ExperiencePoint;

    [BoxGroup("Enemy Specific", CenterLabel = true)]
    public int SkullAmount;

    [SerializeField] public Material Material;

    [Button]
    public override void Reset()
    {
        AttackDamage = 25;
        Health = 100;
        AttackSpeed = 1.5f;
        MovementSpeed = 3;

        ExperiencePoint = 10;
        SkullAmount = 10;
    }
}