using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OutlinesPack
{
    public class EdgeOutlinesPass : ScriptableRenderPass
    {
        private readonly Material material;
        private RenderTargetIdentifier cameraColorTarget;
        private RenderTargetHandle temporaryBuffer;



        public EdgeOutlinesPass(RenderPassEvent renderPassEvent, EdgeOutlinesSettings edgeOutlinesSettings, string shaderPath = "Shader Graphs/EdgeDetection")
        {
            material = new Material(Shader.Find(shaderPath));
            material.SetFloat("_Thickness", edgeOutlinesSettings.Width);
            material.SetColor("_Color", edgeOutlinesSettings.Color);
            material.SetFloat("_DepthThreshold", edgeOutlinesSettings.DepthThreshold);
            material.SetFloat("_NormalThreshold", edgeOutlinesSettings.NormalThreshold);
            material.SetFloat("_Depth_Fadeout_Hardness", edgeOutlinesSettings.DepthFadeoutHardness);
            material.SetFloat("_Depth_Fadeout_Threshold", edgeOutlinesSettings.DepthFadeoutThreshold);
            material.SetInt("Use_Depth_Fadeout", edgeOutlinesSettings.UseDepthFadeout ? 1 : 0);
            material.SetInt("Depth_Fadeout_Preview", edgeOutlinesSettings.DepthFadeoutPreview ? 1 : 0);


        }

        public void Setup(ScriptableRenderer renderer)
        {
			ConfigureInput(ScriptableRenderPassInput.Normal);

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!material) return;

				var renderer = renderingData.cameraData.renderer;


            cameraColorTarget = renderer.cameraColorTarget;


            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler(
                "_EdgeDetecionOutlinesBlit")))
            {
                RenderTextureDescriptor opaqueDescriptor = renderingData.cameraData.cameraTargetDescriptor;

                cmd.GetTemporaryRT(temporaryBuffer.id, opaqueDescriptor, FilterMode.Point);
                Blit(cmd, cameraColorTarget, temporaryBuffer.Identifier(), material, 0);
                Blit(cmd, temporaryBuffer.Identifier(), cameraColorTarget);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }


		public override void OnCameraCleanup(CommandBuffer cmd)
        {
           cmd.ReleaseTemporaryRT(temporaryBuffer.id);
        }
    }

}