using System;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyBehavior
{
    public class EnemyModel : BaseModel
    {
        public NavMeshAgent NavMeshAgent { get; }
        public EnemyStatsSO EnemyStatsSo => BaseStatsSo as EnemyStatsSO;
        public LocalStatHolder LocalStats { get; }

        public EnemyModel(Animator animator, BaseStatsSO baseStatsSo, NavMeshAgent navMeshAgent,
            LocalStatHolder localStats) : base(animator,
            baseStatsSo)
        {
            NavMeshAgent = navMeshAgent;
            LocalStats = localStats;
        }


        [Serializable]
        public class LocalStatHolder
        {
            [field: SerializeField] public float FollowRange { get; set; }
            [field: SerializeField] public float AttackRange { get; set; }
            [field: SerializeField] public float Health { get; set; }
            [field: SerializeField] public double Guid { get; set; }

            public void InitializeLocalData(EnemyStatsSO enemyStatsSo, float attackRange)
            {
                Health = enemyStatsSo.Health;
                AttackRange = attackRange;
            }
        }
    }
}