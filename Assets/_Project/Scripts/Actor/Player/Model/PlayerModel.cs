using System;
using UnityEngine;

namespace PlayerBehaviors
{
    public class PlayerModel : BaseModel
    {
        public PlayerModel(Rigidbody rigidBody, Animator animator, BaseStatsSO baseStatsSo,
            LocalStatHolder localStatHolder) :
            base(animator, baseStatsSo)
        {
            RigidBody = rigidBody;
            LocalStats = localStatHolder;
        }

        public LocalStatHolder LocalStats { get; }

        public PlayerStatsSO PlayerStatsSo => BaseStatsSo as PlayerStatsSO;

        public Rigidbody RigidBody { get; }


        public void UpdateLocalHealth()
        {
            LocalStats.UpdateHealth(PlayerStatsSo.Health);
        }

        [Serializable]
        public class LocalStatHolder
        {
            [field: SerializeField] public float Health { get; set; }

            public void UpdateHealth(float health)
            {
                Health = health;
            }
        }
    }
}