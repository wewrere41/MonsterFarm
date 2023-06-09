using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// TODO: Remove for URP 13.
// https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@13.1/manual/upgrade-guide-2022-1.html
#pragma warning disable CS0618

namespace FlatKit
{
    internal class BlitTexturePass : ScriptableRenderPass
    {
        public static readonly string CopyEffectShaderName = "Hidden/FlatKit/CopyTexture";

        private readonly ProfilingSampler _profilingSampler = new ProfilingSampler("Copy Texture");
        private readonly Material _effectMaterial;
        private readonly Material _copyMaterial;
        private RenderTargetHandle _temporaryColorTexture;

        public BlitTexturePass(Material effectMaterial, Material copyMaterial)
        {
            _effectMaterial = effectMaterial;
            _copyMaterial = CoreUtils.CreateEngineMaterial(CopyEffectShaderName);
        }

        public void Setup(bool useDepth, bool useNormals, bool useColor)
        {
#if UNITY_2020_3_OR_NEWER
            ConfigureInput(
                (useColor ? ScriptableRenderPassInput.Color : ScriptableRenderPassInput.None) |
                (useDepth ? ScriptableRenderPassInput.Depth : ScriptableRenderPassInput.None) |
                (useNormals ? ScriptableRenderPassInput.Normal : ScriptableRenderPassInput.None)
            );
#endif
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureTarget(new RenderTargetIdentifier(renderingData.cameraData.renderer.cameraColorTarget, 0,
                CubemapFace.Unknown, -1));
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_effectMaterial == null) return;
            if (renderingData.cameraData.camera.cameraType != CameraType.Game) return;

            _temporaryColorTexture = new RenderTargetHandle();

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
                // descriptor.depthBufferBits = 0;
                SetSourceSize(cmd, descriptor);

#if UNITY_2022_1_OR_NEWER
            var cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
#else
                var cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTarget;
#endif
                cmd.GetTemporaryRT(_temporaryColorTexture.id, descriptor);

                // Also seen as `renderingData.cameraData.xr.enabled` and `#if ENABLE_VR && ENABLE_XR_MODULE`.

                _effectMaterial.DisableKeyword("_USE_DRAW_PROCEDURAL");
                // Note: `FinalBlitPass` has `cmd.SetRenderTarget` at this point, but it's unclear what that does.
                cmd.Blit(cameraTargetHandle, _temporaryColorTexture.Identifier(), _effectMaterial, 0);
                cmd.Blit(_temporaryColorTexture.Identifier(), cameraTargetHandle);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        // Copied from `PostProcessUtils.cs`.
        private static void SetSourceSize(CommandBuffer cmd, RenderTextureDescriptor desc)
        {
            float width = desc.width;
            float height = desc.height;
            if (desc.useDynamicScale)
            {
                width *= ScalableBufferManager.widthScaleFactor;
                height *= ScalableBufferManager.heightScaleFactor;
            }

            cmd.SetGlobalVector("_SourceSize", new Vector4(width, height, 1.0f / width, 1.0f / height));
        }
    }
}