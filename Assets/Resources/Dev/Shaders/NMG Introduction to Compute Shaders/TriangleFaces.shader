Shader "Tutorial/TriangleFaces"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}

		Pass
		{
			Name "Forward Lit"
			Tags { "Lightmode" = "UniversalForward"}

			HLSLPROGRAM
			//Signal that the shader requires compute buffers
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 5.0

			//Lighting and shadow keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITION_LIGHTS_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT

			//Register our function
			#pragma vertex Vertex
			#pragma fragment Fragment

			#include "TriangleFaces.hlsl"

			ENDHLSL
		}
		
		Pass
		{
			Name "ShadowCaster"
			Tags { "Lightmode" = "ShadowCaster"}

			HLSLPROGRAM
			//Signal that the shader requires compute buffers
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 5.0

			//Lighting and shadow keywords
			#pragma multi_compile_shadowcaster

			//Register our function
			#pragma vertex Vertex
			#pragma fragment Fragment

			#define SHADOW_CASTER_PASS

			#include "TriangleFaces.hlsl"

			ENDHLSL
		}
	}
}
