using System.Runtime.InteropServices;
using UnityEngine;

namespace Utilities.Extensions
{
    public static class RigidbodyExtensions
    {
        public static void ChangeVelocity(this Rigidbody rb, [Optional] float? x, [Optional] float? y,
            [Optional] float? z)

        {
            var velocity = rb.velocity;
            velocity = new Vector3(x ?? velocity.x, y ?? velocity.y,
                z ?? velocity.z);
            rb.velocity = velocity;
        }
    }
}