using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KawaseBlurFeature : ScriptableRendererFeature
{
[System.Serializable]
    public class KawaseBlurSettings {
        public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;
 
        public Material postProcessMaterial = null;
        public int MaterialPassIndex = -1;
        public Target destination = Target.Color;
        public string textureId = "_KawaseBlurPassTexture";
    }
 
    public enum Target {
        Color,
        Texture
    }
 
    public KawaseBlurSettings settings = new KawaseBlurSettings();
    RenderTargetHandle m_RenderTextureHandle;
 
    KawaseBlurPass blitPass;
 
    public override void Create() {
        var passIndex = settings.postProcessMaterial != null ? settings.postProcessMaterial.passCount - 1 : 1;
        settings.MaterialPassIndex = Mathf.Clamp(settings.MaterialPassIndex, -1, passIndex);
        blitPass = new KawaseBlurPass(settings.Event, settings.postProcessMaterial, settings.MaterialPassIndex, name);
        m_RenderTextureHandle.Init(settings.textureId);
    }
 
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        var src = renderer.cameraColorTarget;
        var dest = (settings.destination == Target.Color) ? RenderTargetHandle.CameraTarget : m_RenderTextureHandle;
 
        if (settings.postProcessMaterial == null) {
            Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
            return;
        }
 
        blitPass.Setup(src, dest);
        renderer.EnqueuePass(blitPass);
    }
}
