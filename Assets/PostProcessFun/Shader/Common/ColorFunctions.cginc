#ifndef COLORFUNCTIONS
#define COLORFUNCTIONS

#include "UnityCG.cginc"

float3 hueMode(float3 col, half lumAdd, half satAdjust){
    
    float hue;
    col = pow(col,2.2);
    float minCol = min(min(col.r, col.g), col.b);
    float maxCol = max(max(col.r, col.g), col.b);
    float d = maxCol-minCol;

    if(col.r > col.g && col.r > col.b){
        hue = ((col.g - col.b)/d)%6;
    }
    else if(col.g > col.r && col.g > col.b){
        hue = 2+(col.b - col.r)/d;
    }
    else{
        hue = 4+(col.r - col.g)/d;
    }
    hue *= 60.0;

    half lum = (maxCol + minCol)/2;
    lum *= lumAdd;
    half sat = 0;
    if(d > 0.025){
        sat = d/(1 - abs(2*lum-1));
    }
    sat *= satAdjust;

    lum = saturate(lum);
    sat = saturate(sat);

    half c = (1-abs(2*lum-1))*sat;
    half x = c * (1 - abs((hue/60)%2 - 1));
    half m = lum - c*0.5;

    half3 convertCol = half3(0,0,0);
    
    if(hue > 0.0 && hue < 60.0){
        convertCol = float3(c+m, x+m, m);
    }
    else if(hue > 60.0 && hue < 120.0){
        convertCol = float3(x+m, c+m, m);
    }
    else if(hue > 120.0 && hue < 180.0){
        convertCol = float3(m, c+m, x+m);
    }
    else if(hue > 180.0 && hue < 240.0){
        convertCol = float3(m, x+m, c+m);
    }
    else if(hue > 240.0 && hue < 300.0){
        convertCol = float3(x+m, m, c+m);
    }
    else{
        convertCol = float3(c+m, m, x+m);
    }
    return convertCol;
}

#endif