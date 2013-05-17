Shader "Hidden/cubeShader" {
	Properties {
		_CubeStatic ("Cubemap", Cube) = "" { TexGen CubeReflect }
	}
 
	SubShader {
//		Tags { "Queue"="Background" "IgnoreProjector"="True" "RenderType"="Background" }
		Cull Front
		ZWrite Off
//		ZTest LEqual
		
//		Pass {
//		
//		CGPROGRAM
//			#pragma vertex vert
//			#pragma fragment frag
//			#pragma fragmentoption ARB_precision_hint_fastest
//			#include "UnityCG.cginc"
//
//			struct v2f {
//				float4 pos : SV_POSITION;
//				half3 worldNormal: TEXCOORD0;
//			};
//
//			v2f vert(appdata_base v)
//			{
//				v2f o;
//				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
//				o.worldNormal = v.normal;
//				return o; 
//			}
//
//			samplerCUBE _CubeStatic;
//
//			half4 frag (v2f i) : COLOR
//			{
//				return texCUBE(_CubeStatic, i.worldNormal);
//			}
//		ENDCG  
//		} 
		
		Pass {
		
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				half3 worldNormal: TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
				o.worldNormal = v.normal;
				return o; 
			}

			samplerCUBE _CubeStatic;

			half4 frag (v2f i) : COLOR
			{
				half4 cube = texCUBE(_CubeStatic, i.worldNormal);
				cube.rgb = cube.rgb * cube.a * 8.0f;
				
				return cube;
			}
		ENDCG  
		}
	}
	
	FallBack off
	
}