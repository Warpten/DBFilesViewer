// SkyVertex.hlsl

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

cbuffer MatrixBuffer : register(b1)
{
    float4 translation;
};

struct VertexInput
{
    float3 position : POSITION0;
    float2 texCoord : TEXCOORD0;
};

struct VertexOutput
{
    float4 position : SV_Position;
    float2 texCoord : TEXCOORD0;
};

VertexOutput main(VertexInput input)
{
    float4 position = float4(input.position + translation.xyz, 1.0);
    position = mul(position, matView);
    position = mul(position, matProj);

    VertexOutput output = (VertexOutput) 0;
    output.position = position;
    output.texCoord = input.texCoord;

    return output;
}
