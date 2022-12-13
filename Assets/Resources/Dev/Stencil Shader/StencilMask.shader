Shader "Stencil/StencilMask"
{
	Properties
	{
		[IntRange] _StencilRef ("Stencil Ref", Range(0,255)) = 0
	}
	SubShader
	{
		//URP materials have a Renderqueue of +50, just because.
		//Remember to try editing the queue when the shader stops working
		Tags { "Queue"="Geometry+51" "RenderType"="Opaque" "ForceNoShadowCasting" = "True"}

		Blend Zero One
		ColorMask 0
		ZWrite Off
		ZTest Less

		Offset -1, 0
		//Offset just works, no clue why
		//https://docs.unity3d.com/Manual/SL-CullAndDepth.html
		//https://www.unity3dtips.com/unity-z-fighting-solutions/
		//https://docs.microsoft.com/en-us/windows/win32/direct3d11/d3d10-graphics-programming-guide-output-merger-stage-depth-bias
		//https://www.khronos.org/opengl/wiki/Parameters_of_Polygon_Offset 
		//https://docs.huihoo.com/opengl/glspec1.1/node58.html

		Stencil
		{
			Ref [_StencilRef]
			Comp Always
			Pass Replace
			//ZFail Zero
		}

		Pass
		{

		}
	}
}
