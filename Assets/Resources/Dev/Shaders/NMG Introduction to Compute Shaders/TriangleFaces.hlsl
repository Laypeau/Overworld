#ifndef TRIANGLEFACES_INCLUDED
#define TRIANGLEFACES_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "NMGGraphicsHelpers.hlsl"

struct VertexOut
{
	float3 position; //World space
	float2 uv;
};

struct TriangleOut
{
	VertexOut vertices[3];
	float3 normal; //all vertices in the triangle will share the normal
};

StructuredBuffer<TriangleOut> drawTriangles;

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
float4 _MainTex_ST;

struct VertexOutput
{
	float3 positionWS	: TEXCOORD0;
	float3 normalWS		: TEXCOORD1;
	float2 uv			: TEXCOORD2;
	float4 positionCS	: SV_POSITION;
};

VertexOutput Vertex(uint vertexID : SV_VERTEXID)
{
	VertexOutput output = (VertexOutput)0;

	//TriangleOut contains 3 vertices and a normal. Vertex function iterates over ever vertex (i think), so a new triangle is read every 3 goes
	TriangleOut tri = drawTriangles[vertexID / 3];
	VertexOut input = tri.vertices[vertexID % 3];

	output.positionWS = input.position;
	output.normalWS = tri.normal;
	output.uv = TRANSFORM_TEX(input.uv, _MainTex);
	output.positionCS = CalculatePositionCSWithShadowCasterLogic(input.position, tri.normal);
	
	return output;
}

float4 Fragment(VertexOutput input) : SV_TARGET
{
#ifdef SHADOW_CASTER_PASS
	//if in shadow caster pass, return now. This should be enough to signal that it will cast a shadow - NMG. What does this mean
	return 0;
#else
	InputData lightingInput = (InputData)0;
	lightingInput.positionWS = input.positionWS;
	lightingInput.normalWS = input.normalWS;
	lightingInput.viewDirectionWS = GetViewDirectionFromPosition(input.positionWS);
	lightingInput.shadowCoord = CalculateShadowCoord(input.positionWS, input.positionCS);

	//read main texture
	float3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb;

	//URP's simple lighting function. lightingInput, albedo colour, specular colour, smoothness colour, emission colour, and alpha
	return UniversalFragmentBlinnPhong(lightingInput, albedo, 1, 0, 0, 1);
#endif
}
#endif