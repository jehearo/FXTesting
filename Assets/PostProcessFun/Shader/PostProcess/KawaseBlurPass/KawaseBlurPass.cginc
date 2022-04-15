#ifndef KAWASEBLURPASS
#define KAWASEBLURPASS

#include "UnityCG.cginc"

struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            //Add samplers and vertex parameters
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;
            float _Offset;
            CBUFFER_START(UnityPerMaterial)
            //Add custom parameters
            
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //pixelate uv so that we get a pixel look
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 res = _MainTex_TexelSize.xy;
    
                fixed4 col;                
                col.rgb = tex2D( _MainTex, i.uv ).rgb;
                col.rgb += tex2D( _MainTex, i.uv + float2( _Offset, _Offset ) * res ).rgb;
                col.rgb += tex2D( _MainTex, i.uv + float2( _Offset, -_Offset ) * res ).rgb;
                col.rgb += tex2D( _MainTex, i.uv + float2( -_Offset, _Offset ) * res ).rgb;
                col.rgb += tex2D( _MainTex, i.uv + float2( -_Offset, -_Offset ) * res ).rgb;
                col.rgb /= 5.0f;
                
                return col;
            }
#endif