Shader "Hidden/gaussBlur" {
	Properties 
	{
		_MainTex("Base (RGB)", 2D) = "black" {} 
	}

CGINCLUDE	
#include "UnityCG.cginc"
	
sampler2D _MainTex;	
half blurSize = 1/256.0f;

half4 GaussBlurVertical (v2f_img i) : COLOR
{   
   half4 color = 0.0f;
 
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y - 4.0*blurSize)) * 0.05;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y - 3.0*blurSize)) * 0.09;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y - 2.0*blurSize)) * 0.12;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y - blurSize)) * 0.15;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y)) * 0.16;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y + blurSize)) * 0.15;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y + 2.0*blurSize)) * 0.12;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y + 3.0*blurSize)) * 0.09;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y + 4.0*blurSize)) * 0.05;
 
   return color;
}

half4 GaussBlurHorizontal (v2f_img i) : COLOR
{   
   half4 color = 0.0f;

   color += tex2D(_MainTex, half2(i.uv.x - 4.0*blurSize, i.uv.y)) * 0.05;
   color += tex2D(_MainTex, half2(i.uv.x - 3.0*blurSize, i.uv.y)) * 0.09;
   color += tex2D(_MainTex, half2(i.uv.x - 2.0*blurSize, i.uv.y)) * 0.12;
   color += tex2D(_MainTex, half2(i.uv.x - blurSize, i.uv.y)) * 0.15;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y)) * 0.16;
   color += tex2D(_MainTex, half2(i.uv.x + blurSize, i.uv.y)) * 0.15;
   color += tex2D(_MainTex, half2(i.uv.x + 2.0*blurSize, i.uv.y)) * 0.12;
   color += tex2D(_MainTex, half2(i.uv.x + 3.0*blurSize, i.uv.y)) * 0.09;
   color += tex2D(_MainTex, half2(i.uv.x + 4.0*blurSize, i.uv.y)) * 0.05;
 
   return color;
}


	ENDCG
			
//-----------------------------------------------------------------------------
// Techniques
//-----------------------------------------------------------------------------
Subshader { 
  
Pass {
//	  ZTest Always Cull Off ZWrite Off 
//	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment GaussBlurVertical
      ENDCG
  }  
Pass {
//	  ZTest Always Cull Off ZWrite Off 
//	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment GaussBlurHorizontal
      ENDCG
  }  
  
}
Fallback off
}