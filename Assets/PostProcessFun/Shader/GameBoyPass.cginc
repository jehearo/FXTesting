#ifndef GAMEBOYPASS
#define GAMEBOYPASS

#include "UnityCG.cginc"
#include "Sobel.cginc"

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
            sampler2D _CameraDepthTexture;
            float4 _MainTex_TexelSize;
            float _BitAmount;
            
            CBUFFER_START(UnityPerMaterial)
            //Add custom parameters
            float _Thickness;
            float4 _EdgeColor;

            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //pixelate uv so that we get a pixel look
                o.uv = v.uv;
                return o;
            }

            fixed4 sobel_frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                //Do Sobel Filtering
                float3 edge = step(0.01,sobelDepth(i.uv, _CameraDepthTexture, _MainTex_TexelSize.xy*_Thickness));
                col.rgb = lerp(col.rgb, _EdgeColor, edge);

                return col;
            }

            fixed4 pixel_frag (v2f i) : SV_Target
            {
                fixed4 col = float4(0,0,0,1);

                // just invert the colors
                half2 pixelUV = floor(i.uv*_BitAmount)/_BitAmount;
                i.uv = pixelUV;
                col.rgb = tex2D(_MainTex, i.uv);
                return col;
            }


#endif