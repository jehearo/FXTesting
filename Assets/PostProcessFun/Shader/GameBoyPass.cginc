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
            sampler2D _PixelTex;
            float4 _MainTex_TexelSize;
            float _BitAmount;
            
            CBUFFER_START(UnityPerMaterial)
            //Add custom parameters
            float _Thickness;
            float4 _EdgeColor;
            float _LumaAdd;
            float _SobelStep;
            float4 _Darkest, _Dark, _Ligt, _Ligtest;
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
                float3 edge = step(_SobelStep,sobelDepth(i.uv, _CameraDepthTexture, _MainTex_TexelSize.xy*_Thickness)); 
                col.rgb = lerp(col.rgb, _EdgeColor, edge);

                return col;
            }

            fixed4 posterize_frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                //calculate luminosity based off of sRGB values
                half luma = dot(col.rgb, half3(0.2126, 0.7152, 0.0722)) + _LumaAdd;
                float posterized = floor(luma * 4) / (4 - 1);
                float lumaTimesThree = posterized * 3.0;

                float darkest = saturate(lumaTimesThree);
                float4 color = lerp(_Darkest, _Dark, darkest);

                float light = saturate(lumaTimesThree - 1.0);
                color = lerp(color, _Ligt, light);

                float lightest = saturate(lumaTimesThree - 2.0);
                color = lerp(color, _Ligtest, lightest);
                return color;
            }

            fixed4 pixel_frag (v2f i) : SV_Target
            {
                fixed4 col = float4(0,0,0,1);

                // just invert the colors
                half2 pixelUV = floor(i.uv*_BitAmount)/_BitAmount;
                col.rgb = tex2D(_MainTex, pixelUV);
                half pixelTex = tex2D(_PixelTex, i.uv*_BitAmount);
                col.rgb = lerp(float3(0,0,0), col.rgb, pixelTex);

                return col;
            }


#endif