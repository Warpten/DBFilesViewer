// M2VertexInstancedOld.hlsl

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

cbuffer AnimationMatrices : register(b1)
{
    row_major float4x4 Bones[256];
}

cbuffer PerModelPassBuffer : register(b2)
{
    row_major float4x4 uvAnimation;
    row_major float4x4 uvAnimation2;
    row_major float4x4 uvAnimation3;
    row_major float4x4 uvAnimation4;

    float4 modelPassParams; // x = unlit, y = unfogged, z = alphakey
    float4 animatedColor;

    row_major float4x4 modelPosition;
}

struct VertexInput
{
    float3 position : POSITION0;
    float4 boneWeights : BLENDWEIGHT0;
    int4 bones : BLENDINDEX0;
    float3 normal : NORMAL0;
    float2 texCoord1 : TEXCOORD0;
    float2 texCoord2 : TEXCOORD1;

    float4 mat0 : TEXCOORD2;
    float4 mat1 : TEXCOORD3;
    float4 mat2 : TEXCOORD4;
    float4 mat3 : TEXCOORD5;
};

struct VertexOutput
{
    float4 position : SV_Position;
    float3 normal : NORMAL0;
    float2 texCoord1 : TEXCOORD0;
    float2 texCoord2 : TEXCOORD1;
    float depth : TEXCOORD2;
    float3 worldPosition : TEXCOORD3;
    float4 color : COLOR0;
};

VertexOutput main(VertexInput input)
{
    float4x4 matInstance = float4x4(input.mat0, input.mat1, input.mat2, input.mat3);

    float3x3 matNormal = (float3x3)matInstance;

    float4 basePosition = float4(input.position, 1.0);
    float4 position = mul(basePosition, Bones[input.bones.x]) * input.boneWeights.x;
    position += mul(basePosition, Bones[input.bones.y]) * input.boneWeights.y;
    position += mul(basePosition, Bones[input.bones.z]) * input.boneWeights.z;
    position += mul(basePosition, Bones[input.bones.w]) * input.boneWeights.w;

    float3 normal = float3(0, 0, 0);
    normal += mul(input.normal, (float3x3)Bones[input.bones.x]) * input.boneWeights.x;
    normal += mul(input.normal, (float3x3)Bones[input.bones.y]) * input.boneWeights.y;
    normal += mul(input.normal, (float3x3)Bones[input.bones.z]) * input.boneWeights.z;
    normal += mul(input.normal, (float3x3)Bones[input.bones.w]) * input.boneWeights.w;
    
    position = mul(position, modelPosition);
    position = mul(position, matInstance);
    normal = mul(normal, matNormal);

    float4 worldPos = position;
    position = mul(position, matView);
    position = mul(position, matProj);

    VertexOutput output = (VertexOutput) 0;
    output.position = position;
    output.depth = distance(worldPos, eyePosition);
    output.normal = normal;
    float4 tcTransform = mul(float4(input.texCoord1, 0, 1), uvAnimation);
    output.texCoord1 = tcTransform.xy / tcTransform.w;
    output.texCoord2 = input.texCoord2;
    output.worldPosition = worldPos;
    
    return output;
}
