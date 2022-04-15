using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class KawaseBlurPass : ScriptableRenderPass
{
  public enum RenderTarget {
            Color,
            RenderTexture,
        }
 
        public Material KawaseBlurMaterial = null;
        public int blitShaderPassIndex = 0;
        public FilterMode filterMode { get; set; }
 
        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }
 
        int tmpId1;
        int tmpId2;

        RenderTargetIdentifier tmpRT1;
        RenderTargetIdentifier tmpRT2;


        string m_ProfilerTag;
        static int globalTarget = Shader.PropertyToID("_KawaseBlurPassTexture");
        KawaseBlurData kawaseBlurData;

        public KawaseBlurPass(RenderPassEvent renderPassEvent, Material blitMaterial, int blitShaderPassIndex, string tag) {
            this.renderPassEvent = renderPassEvent;
            this.KawaseBlurMaterial = blitMaterial;
            this.blitShaderPassIndex = blitShaderPassIndex;
            m_ProfilerTag = tag;
        }
        internal static class ShaderConstants
        {
            public static readonly int _Offset = Shader.PropertyToID("_Offset");
        }
        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination) {
            this.source = source;
            this.destination = destination;
            var stack = VolumeManager.instance.stack;
            kawaseBlurData = stack.GetComponent<KawaseBlurData>();
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var width = cameraTextureDescriptor.width / kawaseBlurData.downsample.value;
            var height = cameraTextureDescriptor.height / kawaseBlurData.downsample.value;

            tmpId1 = Shader.PropertyToID("tmpBlurRT1");
            tmpId2 = Shader.PropertyToID("tmpBlurRT2");
            cmd.GetTemporaryRT(tmpId1, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            cmd.GetTemporaryRT(tmpId2, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);

            tmpRT1 = new RenderTargetIdentifier(tmpId1);
            tmpRT2 = new RenderTargetIdentifier(tmpId2);
            
            ConfigureTarget(tmpRT1);
            ConfigureTarget(tmpRT2);
        }

         
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            
            //set shader constants
            //KawaseBlurMaterial.SetFloat(ShaderConstants._Offset, kawaseBlurData.offset.value);
            
            KawaseBlurMaterial.SetFloat(ShaderConstants._Offset, kawaseBlurData.offset.value);
            cmd.Blit(source, tmpRT1, KawaseBlurMaterial);

            for (var i=1; i<kawaseBlurData.passes.value-1; i++) {
                KawaseBlurMaterial.SetFloat(ShaderConstants._Offset, kawaseBlurData.offset.value -1 + i);
                cmd.Blit(tmpRT1, tmpRT2, KawaseBlurMaterial);

                // pingpong
                var rttmp = tmpRT1;
                tmpRT1 = tmpRT2;
                tmpRT2 = rttmp;
            }

            // final pass
            KawaseBlurMaterial.SetFloat(ShaderConstants._Offset, kawaseBlurData.offset.value + kawaseBlurData.passes.value - 2f);
            if (destination == RenderTargetHandle.CameraTarget) {
                cmd.Blit(tmpRT1, source, KawaseBlurMaterial);
            } else {
                cmd.Blit(tmpRT1, tmpRT2, KawaseBlurMaterial);
                cmd.SetGlobalTexture(globalTarget, tmpRT2);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
         
        public override void FrameCleanup(CommandBuffer cmd) {
            
            cmd.ReleaseTemporaryRT(tmpId1);
            cmd.ReleaseTemporaryRT(tmpId2);
        }

}
