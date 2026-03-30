using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Experimental.Rendering;
using System.Collections.Generic;

public class PushPopLayerRenderFeature : ScriptableRendererFeature
{
    [Header("Injection Points")]
    public RenderPassEvent pushEvent = RenderPassEvent.AfterRenderingPostProcessing;
    public RenderPassEvent popEvent = RenderPassEvent.AfterRenderingPostProcessing + 50;

    [Header("Blend")]
    public Material blendMaterial;

    // =========================================================
    // STACK STORAGE
    // =========================================================
    class LayerStack : ContextItem
    {
        public Stack<TextureHandle> stack = new();

        public override void Reset()
        {
            stack.Clear();
        }
    }

    // =========================================================
    // PUSH PASS (SAVE CURRENT FRAMEBUFFER)
    // =========================================================
    class PushPass : ScriptableRenderPass
    {
        public Material material;

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (material == null) return;

            var resources = frameData.Get<UniversalResourceData>();
            var stack = frameData.GetOrCreate<LayerStack>();

            // Save current color buffer
            stack.stack.Push(resources.cameraColor);

            // Create new render target
            var desc = resources.cameraColor.GetDescriptor(renderGraph);
            desc.name = $"_Layer_{stack.stack.Count}";
            desc.clearBuffer = true;
            desc.clearColor = Color.clear; // IMPORTANT: tránh bị tối

            var newLayer = renderGraph.CreateTexture(desc);

            // Switch target
            resources.cameraColor = newLayer;

            // Alpha check
            if (!GraphicsFormatUtility.HasAlphaChannel(desc.format))
            {
                Debug.LogWarning("[PushPopLayer] No alpha channel → blending may break!");
            }
        }
    }

    // =========================================================
    // POP PASS (BLEND BACK)
    // =========================================================
    class PopPass : ScriptableRenderPass
    {
        public Material material;
        private const string FBF_KEYWORD = "_USE_FBF";

        public PopPass()
        {
            profilingSampler = new ProfilingSampler("Pop Layer Blend");
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (material == null) return;

            var resources = frameData.Get<UniversalResourceData>();
            var stack = frameData.GetOrCreate<LayerStack>();

            if (stack.stack.Count == 0)
                return;

            var previous = stack.stack.Pop();

            // Disable framebuffer fetch nếu shader có
            material.DisableKeyword(FBF_KEYWORD);

            // Blend current → previous
            var param = new RenderGraphUtils.BlitMaterialParameters(
                resources.cameraColor,
                previous,
                material,
                0
            );

            renderGraph.AddBlitPass(param, "PopLayer_Blend");

            // Restore
            resources.cameraColor = previous;
        }
    }

    // =========================================================
    // INSTANCE
    // =========================================================
    PushPass pushPass;
    PopPass popPass;

    public override void Create()
    {
        pushPass = new PushPass();
        popPass = new PopPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Safety: skip preview / scene view nếu cần
        if (renderingData.cameraData.isPreviewCamera)
            return;

        pushPass.renderPassEvent = pushEvent;
        pushPass.material = blendMaterial;

        popPass.renderPassEvent = popEvent;
        popPass.material = blendMaterial;

        renderer.EnqueuePass(pushPass);
        renderer.EnqueuePass(popPass);
    }
}