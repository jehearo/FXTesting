#ifndef SOBEL
#define SOBEL

#include "UnityCG.cginc"

half sobel(half2 uv, sampler2D _MainTex, half2 texelSize){
    half x = 0;
    half y = 0;

    x += tex2D(_MainTex, uv + float2(-texelSize.x, -texelSize.y)) * -1.0;
    x += tex2D(_MainTex, uv + float2(-texelSize.x,            0)) * -2.0;
    x += tex2D(_MainTex, uv + float2(-texelSize.x,  texelSize.y)) * -1.0;

    x += tex2D(_MainTex, uv + float2( texelSize.x, -texelSize.y)) *  1.0;
    x += tex2D(_MainTex, uv + float2( texelSize.x,            0)) *  2.0;
    x += tex2D(_MainTex, uv + float2( texelSize.x,  texelSize.y)) *  1.0;

    y += tex2D(_MainTex, uv + float2(-texelSize.x, -texelSize.y)) * -1.0;
    y += tex2D(_MainTex, uv + float2(           0, -texelSize.y)) * -2.0;
    y += tex2D(_MainTex, uv + float2( texelSize.x, -texelSize.y)) * -1.0;

    y += tex2D(_MainTex, uv + float2(-texelSize.x,  texelSize.y)) *  1.0;
    y += tex2D(_MainTex, uv + float2(           0,  texelSize.y)) *  2.0;
    y += tex2D(_MainTex, uv + float2( texelSize.x,  texelSize.y)) *  1.0;

    return sqrt(x * x + y * y);
}

half sobelDepth(half2 uv, sampler2D _Depth, half2 texelSize){
    half x = 0;
    half y = 0;

    x += SAMPLE_DEPTH_TEXTURE(_Depth, uv + float2(-texelSize.x, -texelSize.y)) * -1.0;
    x += SAMPLE_DEPTH_TEXTURE(_Depth, uv + float2(-texelSize.x,            0)) * -2.0;
    x += SAMPLE_DEPTH_TEXTURE(_Depth, uv + float2(-texelSize.x,  texelSize.y)) * -1.0;

    x += SAMPLE_DEPTH_TEXTURE(_Depth, uv + float2( texelSize.x, -texelSize.y)) *  1.0;
    x += SAMPLE_DEPTH_TEXTURE(_Depth, uv + float2( texelSize.x,            0)) *  2.0;
    x += SAMPLE_DEPTH_TEXTURE(_Depth, uv + float2( texelSize.x,  texelSize.y)) *  1.0;

    y += SAMPLE_DEPTH_TEXTURE(_Depth, uv + float2(-texelSize.x, -texelSize.y)) * -1.0;
    y += SAMPLE_DEPTH_TEXTURE(_Depth, uv + float2(           0, -texelSize.y)) * -2.0;
    y += SAMPLE_DEPTH_TEXTURE(_Depth, uv + float2( texelSize.x, -texelSize.y)) * -1.0;

    y += SAMPLE_DEPTH_TEXTURE(_Depth, uv + float2(-texelSize.x,  texelSize.y)) *  1.0;
    y += SAMPLE_DEPTH_TEXTURE(_Depth, uv + float2(           0,  texelSize.y)) *  2.0;
    y += SAMPLE_DEPTH_TEXTURE(_Depth, uv + float2( texelSize.x,  texelSize.y)) *  1.0;

    return sqrt(x * x + y * y);
}

#endif