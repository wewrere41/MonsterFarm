namespace Utilities.Extensions
{
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class ShaderExtensions
    {
        #region RENDER MODE

        public enum BlendMode
        {
            Opaque,
            Transparent
        }

        public static void ChangeRenderMode(this Material sharedMaterial, BlendMode blendMode, [Optional] Color color)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    sharedMaterial.SetInt("_ZWrite", 1);
                    sharedMaterial.SetColor("_BaseColor", color);
                    sharedMaterial.DisableKeyword("_ALPHATEST_ON");
                    sharedMaterial.DisableKeyword("_ALPHABLEND_ON");
                    sharedMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    sharedMaterial.renderQueue = -1;
                    break;
                case BlendMode.Transparent:
                    sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    sharedMaterial.SetInt("_ZWrite", 0);
                    sharedMaterial.DisableKeyword("_ALPHATEST_ON");
                    sharedMaterial.EnableKeyword("_ALPHABLEND_ON");
                    sharedMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    var tmpColor = sharedMaterial.color;
                    tmpColor.a = 0.3f;
                    sharedMaterial.SetColor("_BaseColor", tmpColor);
                    sharedMaterial.renderQueue = 3000;
                    break;
            }
        }

        #endregion
    }
}