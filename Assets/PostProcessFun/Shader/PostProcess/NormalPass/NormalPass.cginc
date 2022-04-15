#ifndef NORMALPASS
#define NORMALPASS

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
            sampler2D _CameraDepthTexture;
            
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

                const float2 offset1 = float2(0.0,0.005);
                const float2 offset2 = float2(0.005,0.0);
                
                float depth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv).r);
                float depth1 = Linear01Depth(tex2D(_CameraDepthTexture, i.uv + offset1).r);
                float depth2 = Linear01Depth(tex2D(_CameraDepthTexture, i.uv + offset2).r);
                
                float3 p1 = float3(offset1, depth1 - depth);
                float3 p2 = float3(offset2, depth2 - depth);
                
                float3 normal = cross(p1, p2);
                normal.z = -normal.z;
                
                return float4(normalize(normal),1);

            }
#endif