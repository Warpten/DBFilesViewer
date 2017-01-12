// BoundingBoxVertex.hlsl

cbuffer GlobalParams : register(b0)
{
	row_major float4x4 matView;
	row_major float4x4 matProj;
	float4 viewport;

	float4 ambientLight;
	float4 diffuseLight;

	float4 fogColor;
	// x -> fogStart
	// y -> fogEnd
	// z -> farClip
	float4 fogParams;

	float4 mousePosition;
	float4 eyePosition;
};

struct VertexInput {
	float3 position : POSITION0;
	float3 texCoord : TEXCOORD0;
};

struct VertexOutput {
	float4 position : SV_Position;
	float3 texCoord : TEXCOORD0;
};

VertexOutput main(VertexInput input) {
	VertexOutput output = (VertexOutput)0;
	output.position = mul(float4(input.position, 1), matView);
	output.position = mul(output.position, matProj);
	output.texCoord = input.texCoord;

	return output;
}