Shader "Hidden/shadowShader" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "black" {}
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
	
		half4 frag (v2f_img i) : COLOR
		{
			half2 uv = i.uv;
			
			half2 shadowUv = (uv - 0.5) * 2.0;
			
			half4 color = tex2D(_MainTex, uv);
			half shadow = dot(shadowUv, shadowUv);
			
			color.rgb -= saturate(0.55f - shadow);
			
			return color;
		}
	ENDCG

	}
}

Fallback off

}