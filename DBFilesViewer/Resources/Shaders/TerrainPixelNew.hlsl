// TerrainPixelNew.hlsl

cbuffer GlobalParams : register(b0)
{
    row_major float4x4 matView;
    row_major float4x4 matProj;
    float4 viewport;

    float4 ambientLight;
    float4 diffuseLight;

    float4 fogColor;
    // x -> fogStart
    // y -> fotEnd
    // z -> farClip
    float4 fogParams;

    float4 mousePosition;
    float4 eyePosition;
};

struct PixelInput
{
	float4 position : SV_Position;
	float3 normal : NORMAL0;
	float2 texCoord0 : TEXCOORD0;
	float2 texCoord1 : TEXCOORD1;
	float2 texCoord2 : TEXCOORD2;
	float2 texCoord3 : TEXCOORD3;
	float2 texCoordAlpha : TEXCOORD4;
	float4 color : COLOR0;
	float4 addColor : COLOR1;
	float depth : TEXCOORD5;
	float3 worldPosition : TEXCOORD6;
};

SamplerState alphaSampler : register(s1);
SamplerState colorSampler : register(s0);

Texture2D alphaTexture : register(t0);
Texture2D holeTexture : register(t1);
Texture2D texture0 : register(t2);
Texture2D texture1 : register(t3);
Texture2D texture2 : register(t4);
Texture2D texture3 : register(t5);
Texture2D texture0_s : register(t6);
Texture2D texture1_s : register(t7);
Texture2D texture2_s : register(t8);
Texture2D texture3_s : register(t9);

float3 sunDirection = float3(1, 1, -1);

struct LightConstantData
{
	float3 DiffuseLight;
	float3 AmbientLight;
	float SpecularLight;
};

cbuffer TextureScaleParams : register(b2)
{
	float4 texScales;
	float4 specularFactors;
};

LightConstantData buildConstantLighting(float3 normal, float3 worldPos)
{
	LightConstantData ret = (LightConstantData) 0;
	float3 lightDir = normalize(float3(1, 1, -1));
		normal = normalize(normal);
	float light = dot(normal, -lightDir);
	if (light < 0.0)
		light = 0.0;
	if (light > 0.5)
		light = 0.5 + (light - 0.5) * 0.65;

	ret.DiffuseLight = diffuseLight.rgb * light;
	ret.AmbientLight = ambientLight.rgb;

	float3 v = normalize(eyePosition.xyz - worldPos);
	float3 h = normalize(-lightDir + v);
	ret.SpecularLight = max(0, dot(normal, h));

	return ret;
}

float4 main(PixelInput input) : SV_Target
{
    float4 alpha = alphaTexture.Sample(alphaSampler, input.texCoordAlpha);
    float holeValue = holeTexture.Sample(alphaSampler, input.texCoordAlpha).r;
    if (holeValue < 0.5)
        discard;

    float4 c0 = texture0.Sample(colorSampler, input.texCoord0.yx * texScales.x);
    float4 c1 = texture1.Sample(colorSampler, input.texCoord1.yx * texScales.y);
    float4 c2 = texture2.Sample(colorSampler, input.texCoord2.yx * texScales.z);
    float4 c3 = texture3.Sample(colorSampler, input.texCoord3.yx * texScales.a);

	float4 c0_s = texture0_s.Sample(colorSampler, input.texCoord0 * texScales.x);
	float4 c1_s = texture1_s.Sample(colorSampler, input.texCoord1 * texScales.y);
	float4 c2_s = texture2_s.Sample(colorSampler, input.texCoord2 * texScales.z);
	float4 c3_s = texture3_s.Sample(colorSampler, input.texCoord3 * texScales.a);

	LightConstantData lightData = buildConstantLighting(input.normal, input.worldPosition);
	float3 spc0 = c0_s.a * c0_s.rgb * pow(lightData.SpecularLight, 8) * specularFactors.x;
	float3 spc1 = c1_s.a * c1_s.rgb * pow(lightData.SpecularLight, 8) * specularFactors.y;
	float3 spc2 = c2_s.a * c2_s.rgb * pow(lightData.SpecularLight, 8) * specularFactors.z;
	float3 spc3 = c3_s.a * c3_s.rgb * pow(lightData.SpecularLight, 8) * specularFactors.w;
	
	spc0 += (1 - specularFactors.x) * pow(lightData.SpecularLight, 8);
	spc1 += (1 - specularFactors.y) * pow(lightData.SpecularLight, 8);
	spc2 += (1 - specularFactors.z) * pow(lightData.SpecularLight, 8);
	spc3 += (1 - specularFactors.w) * pow(lightData.SpecularLight, 8);

	c0.rgb *= lightData.DiffuseLight + lightData.AmbientLight + spc0;
	c1.rgb *= lightData.DiffuseLight + lightData.AmbientLight + spc1;
	c2.rgb *= lightData.DiffuseLight + lightData.AmbientLight + spc2;
	c3.rgb *= lightData.DiffuseLight + lightData.AmbientLight + spc3;

	c0.rgb = saturate(c0.rgb);
	c1.rgb = saturate(c1.rgb);
	c2.rgb = saturate(c2.rgb);
	c3.rgb = saturate(c3.rgb);

    float4 color = (1.0 - (alpha.g + alpha.b + alpha.a)) * c0;
    color += alpha.g * c1;
    color += alpha.b * c2;
    color += alpha.a * c3;

    float4 textureColor = color;

    color.rgb *= input.color.bgr * 2;
    color.rgb += input.addColor.bgr * textureColor;
    color.rgb *= alpha.r;
    color.rgb = saturate(color.rgb);

    float fogDepth = input.depth - fogParams.x;
    fogDepth /= (fogParams.y - fogParams.x);
    float fog = 1.0f - pow(saturate(fogDepth), 1.5);

    color.rgb = (1.0 - fog) * fogColor.rgb + fog * color.rgb;
    color.a = holeValue;

    return color;
}
