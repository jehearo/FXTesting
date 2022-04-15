using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class NormalPass : ScriptableRenderPass
{
  public enum RenderTarget {
            Color,
            RenderTexture,
        }
 
        public Material NormalMaterial = null;
        public int blitShaderPassIndex = 0;
        public FilterMode filterMode { get; set; }
 
        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }
 
        RenderTargetHandle m_TemporaryColorTexture1;
        string m_ProfilerTag;
        static int globalTarget = Shader.PropertyToID("_NormalPassTexture");

        public NormalPass(RenderPassEvent renderPassEvent, Material blitMaterial, int blitShaderPassIndex, string tag) {
            this.renderPassEvent = renderPassEvent;
            this.NormalMaterial = blitMaterial;
            this.blitShaderPassIndex = blitShaderPassIndex;
            m_ProfilerTag = tag;
            m_TemporaryColorTexture1.Init("_NormalTemporaryColorTexture1");
        }

        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination) {
            this.source = source;
            this.destination = destination;
            var stack = VolumeManager.instance.stack;
        }
         
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
 
            var camera = renderingData.cameraData.camera;
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = camera.scaledPixelWidth;
            opaqueDesc.height = camera.scaledPixelHeight;
            opaqueDesc.colorFormat = RenderTextureFormat.Default;
            opaqueDesc.depthBufferBits = 16;
            filterMode = FilterMode.Bilinear;

            //NormalMaterial.SetFloat(ShaderConstants._BitAmount, NormalData.bit.value);
            // temporary RT to blit between
                cmd.GetTemporaryRT(m_TemporaryColorTexture1.id, opaqueDesc, filterMode);
                //Do render passes
                cmd.Blit(source, m_TemporaryColorTexture1.Identifier(), NormalMaterial,0);
            // Can't read and write to same color target, use a TemporaryRT
            if (destination == RenderTargetHandle.CameraTarget) {
                
                cmd.Blit(m_TemporaryColorTexture1.Identifier(), source);
                
            } else {
                cmd.Blit(source, source);
                cmd.SetGlobalTexture(globalTarget, m_TemporaryColorTexture1.Identifier());
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
         
        public override void FrameCleanup(CommandBuffer cmd) {
            
              cmd.ReleaseTemporaryRT(m_TemporaryColorTexture1.id);
            
        }

}
