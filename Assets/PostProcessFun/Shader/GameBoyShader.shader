Shader "PostEffect/GameBoyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelTex ("Texture", 2D) = "white" {}
        _Darkest ("Darkest", color) = (0.0588235, 0.21961, 0.0588235)
        _Dark ("Dark", color) = (0.188235, 0.38431, 0.188235)
        _Ligt ("Light", color) = (0.545098, 0.6745098, 0.0588235)
        _Ligtest ("Lightest", color) = (0.607843, 0.7372549, 0.0588235)
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
            #pragma fragment posterize_frag

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
