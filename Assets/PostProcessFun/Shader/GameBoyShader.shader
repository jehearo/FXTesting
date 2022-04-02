Shader "PostEffect/GameBoyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment sobel_frag

            #include "UnityCG.cginc"
            #include "GameBoyPass.cginc"
            
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment pixel_frag

            #include "UnityCG.cginc"
            #include "GameBoyPass.cginc"
            
            ENDCG
        }
    }
}
