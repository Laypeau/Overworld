﻿Shader "Tutorial/Ronja"{
	//show values to edit in inspector
	Properties
	{
		_Color ("Tint", Color) = (0, 0, 0, 1)
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType"="Opaque" "Queue"="Geometry" }

		Pass
		{
			CGPROGRAM		

			//include useful shader functions
			#include "UnityCG.cginc"

			//define vertex and fragment shader functions
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;

			//the mesh data thats read by the vertex shader
			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			//the data thats passed from the vertex to the fragment shader and interpolated by the rasterizer
			struct v2f{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v){
				v2f o;
				//convert the vertex positions from object space to clip space so they can be rendered correctly
				o.position = UnityObjectToClipPos(v.vertex);
				//apply the texture transforms to the UV coordinates and pass them to the v2f struct
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET{
			    //read the texture color at the uv coordinate
				fixed4 col = tex2D(_MainTex, i.uv);
				//multiply the texture color and tint color
				col *= _Color;
				return col;
			}

			ENDCG
		}
	}
	Fallback "VertexLit"
}