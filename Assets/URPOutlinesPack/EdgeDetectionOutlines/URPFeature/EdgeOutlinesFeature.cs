using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace OutlinesPack
{
    public class EdgeOutlinesFeature : ScriptableRendererFeature
    {
        [SerializeField] EdgeOutlinesSettings OutlinesSettings = new EdgeOutlinesSettings();

        [SerializeField] private LayerMask outlinesLayerMask = ~0;
        [SerializeField] private bool useDepthMask = true;

        DepthMaskPass depthMaskPass;
        DepthNormalOnlyPass depthNormalOnlyPass;
        EdgeOutlinesPass edgeOutlinesPass;

        public override void Create()
        {
            // In order to add more edge detection features that one, you need to copy this script, give it another name,
            // copy shader EdgeDetection sahder graph and change _NormalsDepthTex in the new shader graph for the new texture name.
            // You also have to change depth mask name here and in new copy of NormaldSepth.shader

            // In copy of this feature change _SceneMask_Edge to something like _SceneMask_Edge_1 and OutlinesPack/MaskShader to the new OutlinesPack/MaskShader_1 path
            if (useDepthMask) depthMaskPass = new DepthMaskPass(RenderPassEvent.AfterRenderingPrePasses, outlinesLayerMask, "_SceneMask_Edge", "OutlinesPack/MaskShader");

            // In copy of this feature change _NormalsDepthTex to something like _NormalsDepthTex_1 do the same in the reference of the new shader graph property
            depthNormalOnlyPass = new DepthNormalOnlyPass(RenderPassEvent.AfterRenderingPrePasses, outlinesLayerMask, useDepthMask, "_NormalsDepthTex");

            // In copy of this feature change the path to the new shader graph with changed _NormalsDepthTex reference.
            edgeOutlinesPass = new EdgeOutlinesPass(RenderPassEvent.BeforeRenderingPostProcessing, OutlinesSettings , "Shader Graphs/EdgeDetection");

        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (useDepthMask) renderer.EnqueuePass(depthMaskPass);
            renderer.EnqueuePass(depthNormalOnlyPass);
            if (edgeOutlinesPass == null) return;
            edgeOutlinesPass.Setup(renderer);
            renderer.EnqueuePass(edgeOutlinesPass);
        }
    }


}