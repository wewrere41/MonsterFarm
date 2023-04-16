using UnityEngine;

namespace Utilities.Helper
{
    public static class MathfUtilities
    {
        public static float ClampAngle(float current, float min, float max)
        {
            var dtAngle = Mathf.Abs(((min - max) + 180) % 360 - 180);
            var hdtAngle = dtAngle * 0.5f;
            var midAngle = min + hdtAngle;

            var offset = Mathf.Abs(Mathf.DeltaAngle(current, midAngle)) - hdtAngle;
            if (offset > 0)
                current = Mathf.MoveTowardsAngle(current, midAngle, offset);
            return current;
        }

        public static float CalculateDotProduct(Transform lookFrom, Transform lookTo)
        {
            var direction = lookTo.position - lookFrom.position;
            var normal = direction.normalized;
            return Vector3.Dot(normal, lookFrom.forward);
        }

        public static float FastDistance(Vector3 a, Vector3 b)
        {
            var x = a.x - b.x;
            var y = a.y - b.y;
            var z = a.z - b.z;
            return FastSqrt(x * x + y * y + z * z);
        }
        public static float FastSqrt(float x)
        {
            unsafe
            {
                var i = *(uint*)&x;
                // adjust bias
                i += 127 << 23;
                // approximation of square root
                i >>= 1;
                return *(float*)&i;
            }
        }
    }
}