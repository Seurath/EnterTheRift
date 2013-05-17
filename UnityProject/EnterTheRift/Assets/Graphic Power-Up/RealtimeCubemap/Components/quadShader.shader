Shader "Hidden/quadShader" {
Properties {
	_MainTex("Base ", 2D) = "black" {}
}

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
 
	CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
			
		fixed4 frag (v2f_img i) : COLOR
		{
			half2 uv = i.uv;
			uv.y = 1 - uv.y;
		 	return tex2D(_MainTex, uv);
		}
	ENDCG

	}
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

	CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		
		fixed4 frag (v2f_img i) : COLOR
		{
			half2 uv = i.uv;
//			uv.y = 1 - uv.y;
			uv.x = 1 - uv.x;
		 	return tex2D(_MainTex, uv);
		}
	ENDCG

	}
	
}

Fallback off

}