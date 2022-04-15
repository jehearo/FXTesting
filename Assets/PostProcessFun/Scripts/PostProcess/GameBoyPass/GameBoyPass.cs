using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class GameBoyPass : ScriptableRenderPass
{
  public enum RenderTarget {
            Color,
            RenderTexture,
        }
 
        public Material gameBoyMaterial = null;
        public int blitShaderPassIndex = 0;
        public FilterMode filterMode { get; set; }
 
        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }
 
        RenderTargetHandle m_TemporaryColorTexture1;
        RenderTargetHandle m_TemporaryColorTexture2;
        string m_ProfilerTag;

        GameBoyData gameBoyData;
         
        public GameBoyPass(RenderPassEvent renderPassEvent, Material blitMaterial, int blitShaderPassIndex, string tag) {
            this.renderPassEvent = renderPassEvent;
            this.gameBoyMaterial = blitMaterial;
            this.blitShaderPassIndex = blitShaderPassIndex;
            m_ProfilerTag = tag;
            m_TemporaryColorTexture1.Init("_GameBoyTemporaryColorTexture1");
            m_TemporaryColorTexture2.Init("_GameBoyTemporaryColorTexture2");       
            }
        
         internal static class ShaderConstants
        {
            public static readonly int _BitAmount = Shader.PropertyToID("_BitAmount");
            public static readonly int _Thickness = Shader.PropertyToID("_Thickness");
            public static readonly int _EdgeColor = Shader.PropertyToID("_EdgeColor");
            public static readonly int _LumaAdd = Shader.PropertyToID("_LumaAdd");
            public static readonly int _SobelStep = Shader.PropertyToID("_SobelStep");
        }

        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination) {
            this.source = source;
            this.destination = destination;
            var stack = VolumeManager.instance.stack;
            gameBoyData = stack.GetComponent<GameBoyData>();
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

            gameBoyMaterial.SetFloat(ShaderConstants._BitAmount, gameBoyData.bit.value);
            gameBoyMaterial.SetFloat(ShaderConstants._Thickness, gameBoyData.thickness.value);
            gameBoyMaterial.SetColor(ShaderConstants._EdgeColor, gameBoyData.edgecolor.value);
            gameBoyMaterial.SetFloat(ShaderConstants._LumaAdd, gameBoyData.lumaAdd.value);
            gameBoyMaterial.SetFloat(ShaderConstants._SobelStep, gameBoyData.sobelStep.value);
            // Can't read and write to same color target, use a TemporaryRT
            if (destination == RenderTargetHandle.CameraTarget) {
                
                // temporary RT to blit between
                cmd.GetTemporaryRT(m_TemporaryColorTexture1.id, opaqueDesc, filterMode);
                cmd.GetTemporaryRT(m_TemporaryColorTexture2.id, opaqueDesc, filterMode);

                //Do render passes
                //Initial Sobel
                //Get Camera Direction
                cmd.Blit(source, m_TemporaryColorTexture1.Identifier(), gameBoyMaterial,0);

                //If GB colour filter is added 
                if(gameBoyData.gameBoyFilter.value){
                    cmd.Blit(m_TemporaryColorTexture1.Identifier(), m_TemporaryColorTexture2.Identifier(), gameBoyMaterial,2);
                    cmd.Blit(m_TemporaryColorTexture2.Identifier(), m_TemporaryColorTexture1.Identifier(), gameBoyMaterial,1);
                    cmd.Blit(m_TemporaryColorTexture1.Identifier(), source);
                }
                else{
                    cmd.Blit(m_TemporaryColorTexture1.Identifier(), m_TemporaryColorTexture2.Identifier(), gameBoyMaterial,2);
                    cmd.Blit(m_TemporaryColorTexture2.Identifier(), source);
                }
            } else {
                Blit(cmd, source, destination.Identifier(), gameBoyMaterial);
            }
 
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
         
        public override void FrameCleanup(CommandBuffer cmd) {
            if (destination == RenderTargetHandle.CameraTarget){
              cmd.ReleaseTemporaryRT(m_TemporaryColorTexture1.id);
              cmd.ReleaseTemporaryRT(m_TemporaryColorTexture2.id);
            }
        }

}
