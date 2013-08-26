Shader "Hidden/encodeShader" {
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
	
		fixed4 frag (v2f_img i) : COLOR
		{
			half2 uv = i.uv;
			
			half3 color = tex2D(_MainTex, uv).rgb;
			half4 rgbm;
			color *= 1.0 / 8.0;
			rgbm.a = saturate( max( max( color.r, color.g ), max( color.b, 1e-6 ) ) );
			rgbm.a = ceil( rgbm.a * 255.0 ) / 255.0;
			rgbm.rgb = color / rgbm.a;
			return rgbm;
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
			
			half3 color = tex2D(_MainTex, uv).rgb;
			color = pow(color, 1.0f/2.2f);
			half4 rgbm;
			color *= 1.0 / 8.0;
			rgbm.a = saturate( max( max( color.r, color.g ), max( color.b, 1e-6 ) ) );
			rgbm.a = ceil( rgbm.a * 255.0 ) / 255.0;
			rgbm.rgb = color / rgbm.a;
			return rgbm;
		}
	ENDCG

	}
}

Fallback off

}