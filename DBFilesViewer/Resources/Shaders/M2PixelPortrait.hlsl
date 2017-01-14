// M2PixelPortrait.hlsl

Texture2D texture1 : register(t0);
Texture2D texture2 : register(t1);
Texture2D texture3 : register(t2);

SamplerState sampler1 : register(s0);

struct PixelInput
{
    float4 position : SV_Position;
    float3 normal : NORMAL0;
    float2 texCoord1 : TEXCOORD0;
    float2 texCoord2 : TEXCOORD1;
};

cbuffer PerModelPassBuffer : register(b0)
{
    row_major float4x4 uvAnimation;
    row_major float4x4 uvAnimation2;
    row_major float4x4 uvAnimation3;
    row_major float4x4 uvAnimation4;

    float4 modelPassParams; // x = unlit, y = unfogged, z = alphakey
    float4 animatedColor; // animatedColor
    float4 transparency; // transparency for each value

    row_major float4x4 modelPosition;
    float4 alphaRef; // x value, yzw unused
}

float3 getDiffuseLight(float3 normal)
{
    float3 lightDir = float3(-1, 1, -1);
    float light = saturate(dot(normal, normalize(-lightDir)));
    float3 diffuse = float3(1, 1, 1) * light * modelPassParams.x;
    diffuse += float3(1, 1, 1); // No ambient lighting defined
    diffuse = saturate(diffuse);
    return diffuse;
}

/*float3 applyFog(float3 finalColor, PixelInput input)
{
    float fogDepth = input.depth; // - fogParams.x; // No fog defined
    fogDepth /= (fogParams.y - fogParams.x);
    float fog = pow(saturate(fogDepth), 1.5f) * modelPassParams.y;
    return fog * fogColor.rgb + (1.0f - fog) * finalColor.rgb;
}*/

float3 mix(float3 a, float3 b, float c)
{
    return (b - a) * c + a;
}

float4 commonFinalize(float4 finalizeColor, PixelInput input)
{
    // float4 finalizeColor = float4(1.0, 1.0, 1.0, 1.0);
    finalizeColor.rgb *= getDiffuseLight(input.normal);

    clip((modelPassParams.z && finalizeColor.a < alphaRef.x) ? -1 : 1);
    return finalizeColor;
}

float4 main_PS_Combiners_Opaque(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);

    float4 finalColor;
    finalColor.rgb = tex1.rgb * animatedColor.rgb;
    finalColor.a = animatedColor.a;

    return commonFinalize(finalColor, input);
}

float4 main_PS_Combiners_Mod(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);

    float4 finalColor;
    finalColor.rgba = tex1.rgba * animatedColor.rgba;

    return commonFinalize(finalColor, input);
}

// Not on wiki
float4 main_PS_Combiners_Opaque_Alpha(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 combinedColor = float4(1, 1, 1, 1);
    float4 r0 = tex1;
    float4 r1 = tex2;
    r1.rgb = -r0.rgb + r1.rgb;
    r0.rgb = r1.a * r1.rgb + r0.rgb;
    r0.rgb *= animatedColor.rgb;
    r0.rgb *= 2.0f;
    combinedColor.rgb = r0.rgb;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Opaque_Mod(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgb = tex1.rgb * tex2.rgb * animatedColor.rgb;
    finalColor.a = tex2.a * animatedColor.a;

    return commonFinalize(finalColor, input);
}

float4 main_PS_Combiners_Opaque_Mod2x(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgb = tex1.rgb * animatedColor.rgb * tex2.rgb * 2.0;
    finalColor.a = tex2.a * animatedColor.a * 2.0;

    return commonFinalize(finalColor, input);
}

float4 main_PS_Combiners_Opaque_Mod2xNA(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgb = tex1.rgb * animatedColor.rgb * tex2.rgb * 2.0;
    finalColor.a = animatedColor.a;

    return commonFinalize(finalColor, input);
}

float4 main_PS_Combiners_Opaque_Opaque(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgb = tex1.rgb * tex2.rgb * animatedColor.rgb;
    finalColor.a = animatedColor.a;

    return commonFinalize(finalColor, input);
}

float4 main_PS_Combiners_Mod_Mod(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgba = tex1.rgba * tex2.rgba * animatedColor.rgba;

    return commonFinalize(finalColor, input);
}

float4 main_PS_Combiners_Mod_Mod2x(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgba = tex1.rgba * tex2.rgba * animatedColor.rgba * 2.0;

    return commonFinalize(finalColor, input);
}

float4 main_PS_Combiners_Mod_Add(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgba = tex2.rgba + tex1.rgba * animatedColor.rgba;

    return commonFinalize(finalColor, input);
}

float4 main_PS_Combiners_Mod_Mod2xNA(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgb = tex1.rgb * tex2.rgb * animatedColor.rgb * 2.0;
    finalColor.a = tex1.a * animatedColor.a;

    return commonFinalize(finalColor, input);
}

float4 main_PS_Combiners_Mod_AddNA(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgb = tex2.rgb + tex1.rgb * animatedColor.rgb;
    finalColor.a = tex1.a * animatedColor.a;

    return commonFinalize(finalColor, input);
}

float4 main_PS_Combiners_Mod_Opaque(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgb = tex1.rgb * tex2.rgb * animatedColor.rgb;
    finalColor.a = tex1.a;

    return commonFinalize(finalColor, input);
}

float4 main_PS_Combiners_Opaque_AddAlpha(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgb = animatedColor.rgb * tex1.rgb + tex2.rgb * tex2.a;
    finalColor.a = animatedColor.a;

    return commonFinalize(finalColor, input);
}

float4 main_PS_Combiners_Opaque_Alpha_Alpha(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgb = animatedColor.rgb * mix(mix(tex1.rgb, tex2.rgb, tex2.a), tex1.rgb, tex1.a);
    finalColor.a = animatedColor.a;

    return commonFinalize(finalColor, input);
}


float4 main_PS_Combiners_Opaque_AddAlpha_Alpha(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgb = animatedColor.rgb * tex1.rgb + (tex2.rgb * tex2.a * (1.0 - tex1.a));
    finalColor.a = animatedColor.a;

    return commonFinalize(finalColor, input);
}

float4 main_PS_Combiners_Mod_AddAlpha(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgb = animatedColor.rgb * tex1.rgb + tex2.rgb * tex2.a;
    finalColor.a = animatedColor.a * tex1.a;

    return commonFinalize(finalColor, input);
}

// not on wiki
float4 main_PS_Combiners_Mod_Add_Alpha(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 combinedColor;
    float4 r0 = tex1;
    r0.rgb *= animatedColor.rgb;
    r0.rgb += r0.rgb;
    float4 r1 = tex2;
    float4 r2;
    r2.x = -r0.a + 1;
    combinedColor.a = animatedColor.a * r0.a * r1.a;
    r0.rgb = r1.rgb * r2.x + r0.rgb;
    combinedColor.rgb = r0.rgb;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Mod_AddAlpha_Alpha(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float addMapBrightness = (0.300000012 * tex2.r) + (0.589999974 * tex2.g) + (0.109999999 * tex2.b);

    float4 finalColor;
    // Parenthesis mismatch on wiki...
    finalColor.rgb = animatedColor.rgb * tex1.rgb + (tex2.rgb * tex2.a * (1.0 - tex1.a)) + ((tex2.rgb * tex2.a) * (1.0 - tex1.a));
    finalColor.a = animatedColor.a * (tex1.a + (tex2.a * addMapBrightness));

    return commonFinalize(finalColor, input);
}

float4 main_PS_Combiners_Opaque_Mod2xNA_Alpha(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 finalColor;
    finalColor.rgb = animatedColor.rgb * ((tex1.rgb - tex1.rgb * tex2.rgb) * tex1.a + tex1.rgb * tex2.rgb);
    finalColor.a = animatedColor.a;

    return commonFinalize(finalColor, input);
}

// Not on wiki
float4 main_PS_Combiners_Opaque_ModNA_Alpha(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 combinedColor;
    float4 r0 = tex2;
    float4 r1 = tex1;
    float3 r2 = r0.rgb * r1.rgb;
    r0.rgb = -r1.rgb * r0.rgb + r1.rgb;
    r0.rgb = r1.a * r0.rgb + r2.rgb;
    r0.rgb *= animatedColor.rgb;
    r0.rgb *= 2.0f;
    combinedColor.rgb = r0.rgb;
    combinedColor.a = animatedColor.a;

    return commonFinalize(combinedColor, input);
}

// not on wiki
float4 main_PS_Combiners_Opaque_AddAlpha_Wgt(PixelInput input) : SV_Target
{
    float4 tex1 = texture1.Sample(sampler1, input.texCoord1);
    tex1.a *= transparency.x;
    clip((modelPassParams.z && tex1.a < alphaRef.x) ? -1 : 1);
    float4 tex2 = texture2.Sample(sampler1, input.texCoord2);
    tex2.a *= transparency.y;

    float4 combinedColor;
    float4 r0 = tex1;
    r0.rgb *= animatedColor.rgb;
    r0.rgb *= r0.rgb;
    float4 r1 = tex2;
    r1.rgb *= r1.a;
    r0.rgb = r1.rgb + r0.rgb;
    combinedColor.rgb = r0.rgb;
    combinedColor.a = animatedColor.a;

    return commonFinalize(combinedColor, input);
}
