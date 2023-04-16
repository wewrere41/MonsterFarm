using System.Runtime.InteropServices;
using UnityEngine;
using Utilities.Helper;

namespace Utilities.Extensions
{
    public static class TransformExtesions
    {
        #region CHANGE POS WITH AXIS

        public static void ChangePosition(this Transform transform, [Optional] float? x, [Optional] float? y,
            [Optional] float? z)
        {
            var position = transform.position;
            position = new Vector3(x ?? position.x, y ?? position.y,
                z ?? position.z);
            transform.position = position;
        }

        public static void ChangeLocalPosition(this Transform transform, [Optional] float? x, [Optional] float? y,
            [Optional] float? z)
        {
            var localPosition = transform.localPosition;
            localPosition = new Vector3(x ?? localPosition.x, y ?? localPosition.y,
                z ?? localPosition.z);
            transform.localPosition = localPosition;
        }

        #endregion

        #region CLAMP POS WITH AXIS

        public static void Clamp(this Transform transform,
            [Optional] float? xMin, [Optional] float? yMin, [Optional] float? zMin,
            [Optional] float? xMax, [Optional] float? yMax, [Optional] float? zMax)
        {
            var position = transform.position;

            if (xMin != null)
                position.x = Mathf.Clamp(position.x, xMin.Value, xMax ?? -xMin.Value);
            if (yMin != null)
                position.y = Mathf.Clamp(position.y, yMin.Value, yMax ?? -yMin.Value);
            if (zMin != null)
                position.z = Mathf.Clamp(position.z, zMin.Value, zMax ?? -zMin.Value);

            transform.position = position;
        }

        public static void ClampLocal(this Transform transform,
            [Optional] float? xMin, [Optional] float? yMin, [Optional] float? zMin,
            [Optional] float? xMax, [Optional] float? yMax, [Optional] float? zMax)
        {
            var localPosition = transform.localPosition;

            if (xMin != null)
                localPosition.x = Mathf.Clamp(localPosition.x, xMin.Value, xMax ?? -xMin.Value);
            if (yMin != null)
                localPosition.y = Mathf.Clamp(localPosition.y, yMin.Value, yMax ?? -yMin.Value);
            if (zMin != null)
                localPosition.z = Mathf.Clamp(localPosition.z, zMin.Value, zMax ?? -zMin.Value);

            transform.localPosition = localPosition;
        }

        #endregion

        #region CLAMP ANGLE

        public static void ClampEulerAngles(this Transform transform,
            [Optional] float? xMin, [Optional] float? yMin, [Optional] float? zMin,
            [Optional] float? xMax, [Optional] float? yMax, [Optional] float? zMax)
        {
            var tmpEulerAngles = transform.eulerAngles;
            if (xMin != null)
            {
                tmpEulerAngles.x = MathfUtilities.ClampAngle(transform.eulerAngles.x, xMin.Value, xMax ?? xMin.Value);
            }

            if (yMin != null)
            {
                tmpEulerAngles.y = MathfUtilities.ClampAngle(transform.eulerAngles.y, yMin.Value, yMax ?? yMin.Value);
            }

            if (zMin != null)
            {
                tmpEulerAngles.z = MathfUtilities.ClampAngle(transform.eulerAngles.z, zMin.Value, zMax ?? zMin.Value);
            }

            transform.eulerAngles = tmpEulerAngles;
        }

        public static void ClampLocalEulerAngles(this Transform transform,
            [Optional] float? xMin, [Optional] float? yMin, [Optional] float? zMin,
            [Optional] float? XMax, [Optional] float? yMax, [Optional] float? zMax)
        {
            var tmpLocalEulerAngles = transform.localEulerAngles;
            if (xMin != null)
            {
                tmpLocalEulerAngles.x =
                    MathfUtilities.ClampAngle(transform.eulerAngles.x, xMin.Value, XMax ?? xMin.Value);
            }

            if (yMin != null)
            {
                tmpLocalEulerAngles.y =
                    MathfUtilities.ClampAngle(transform.eulerAngles.y, yMin.Value, yMax ?? yMin.Value);
            }

            if (zMin != null)
            {
                tmpLocalEulerAngles.z =
                    MathfUtilities.ClampAngle(transform.eulerAngles.z, zMin.Value, zMax ?? zMin.Value);
            }

            transform.localEulerAngles = tmpLocalEulerAngles;
        }

        #endregion

        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }
}