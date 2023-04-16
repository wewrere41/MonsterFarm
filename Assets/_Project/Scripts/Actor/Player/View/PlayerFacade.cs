using UnityEngine;

namespace PlayerBehaviors
{
    public class PlayerFacade : BaseFacade
    {
        public Rigidbody Rigidbody => (_model as PlayerModel).RigidBody;

        public PlayerStatsSO PlayerStatsSo => (_model as PlayerModel).PlayerStatsSo;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 3);
        }
    }
}