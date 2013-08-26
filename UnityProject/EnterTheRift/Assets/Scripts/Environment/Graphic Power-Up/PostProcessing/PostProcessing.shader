Shader "Hidden/Post Processing" {
	Properties 
	{
		_MainTex("Base (RGB)", 2D) = "white" {} 
		_SecondTex("Base (RGB)", 2D) = "black" {} 
		_BloomTex("Base (RGB)", 2D) = "black" {} 
	}

	CGINCLUDE	
	#include "UnityCG.cginc"

	half  _BrightPassThreshold	= 1.0f;
	half  _BrightPassOffset	= 2.0f;
	static const half3 _LuminanceVec		= half3(0.2125f, 0.7154f, 0.0721f);
		
	uniform half4 avSampleOffsets0;
	uniform half4 avSampleOffsets1;
	uniform half4 avSampleOffsets2;
	uniform half4 avSampleOffsets3;
	uniform half4 avSampleOffsets4;
	uniform half4 avSampleOffsets5;
	uniform half4 avSampleOffsets6;
	uniform half4 avSampleOffsets7;

	uniform half4 avSampleWeights0;
	uniform half4 avSampleWeights1;
	uniform half4 avSampleWeights2;	
	uniform half4 avSampleWeights3;
	uniform half4 avSampleWeights4;
	uniform half4 avSampleWeights5;
	uniform half4 avSampleWeights6;
	uniform half4 avSampleWeights7;
	uniform half4 avSampleWeights8;
	uniform half4 avSampleWeights9;
	uniform half4 avSampleWeights10;
	uniform half4 avSampleWeights11;
	uniform half4 avSampleWeights12;
	uniform half4 avSampleWeights13;
	uniform half4 avSampleWeights14;
	uniform half4 avSampleWeights15;
	
	half  _MiddleGray;
	half  _AdaptaionSpeed;
	half  _GammaValue;
	half  _Offset;
	half  _ElapsedTime;

	half  _BloomScale = 1.0f;
	half  _BloomBlurSizeV = 1.0f/256.0f;
	half  _BloomBlurSizeH = 1.0f/256.0f;
	
	sampler2D _MainTex;	
	sampler2D _SecondTex;
	sampler2D _BloomTex;	
		
half4 SampleLumInitial	(v2f_img i) : COLOR
{
    half3 vSample = 0.0f;
    half  fLogLumSum = 0.0f;

	vSample = tex2D(_MainTex, i.uv+avSampleOffsets0.xy);
	fLogLumSum += log(dot(vSample, _LuminanceVec)+0.0001f);
	vSample = tex2D(_MainTex, i.uv+avSampleOffsets0.zw);
	fLogLumSum += log(dot(vSample, _LuminanceVec)+0.0001f);
	vSample = tex2D(_MainTex, i.uv+avSampleOffsets1.xy);
	fLogLumSum += log(dot(vSample, _LuminanceVec)+0.0001f);
	vSample = tex2D(_MainTex, i.uv+avSampleOffsets1.zw);
	fLogLumSum += log(dot(vSample, _LuminanceVec)+0.0001f);
	vSample = tex2D(_MainTex, i.uv+avSampleOffsets2.xy);
	fLogLumSum += log(dot(vSample, _LuminanceVec)+0.0001f);
	vSample = tex2D(_MainTex, i.uv+avSampleOffsets2.zw);
    fLogLumSum += log(dot(vSample, _LuminanceVec)+0.0001f);
	vSample = tex2D(_MainTex, i.uv+avSampleOffsets3.xy);
	fLogLumSum += log(dot(vSample, _LuminanceVec)+0.0001f);
	vSample = tex2D(_MainTex, i.uv+avSampleOffsets3.zw);
	fLogLumSum += log(dot(vSample, _LuminanceVec)+0.0001f);
	vSample = tex2D(_MainTex, i.uv+avSampleOffsets4.xy);
	fLogLumSum += log(dot(vSample, _LuminanceVec)+0.0001f);

    fLogLumSum /= 9;

    return half4(fLogLumSum, fLogLumSum, fLogLumSum, 1.0f);
}	

half4 SampleLumIterative (v2f_img i) : COLOR
{
    half fResampleSum = 0.0f; 
    
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets0.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets0.zw);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets1.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets1.zw);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets2.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets2.zw);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets3.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets3.zw);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets4.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets4.zw);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets5.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets5.zw);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets6.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets6.zw);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets7.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets7.zw);	
	 
    fResampleSum /= 16;

    return half4(fResampleSum, fResampleSum, fResampleSum, 1.0f);
}

half4 DownScale4x4 (v2f_img i) : COLOR
{	
    half4 vSample = 0.0f;
    
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets0.xy);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets0.zw);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets1.xy);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets1.zw);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets2.xy);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets2.zw);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets3.xy);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets3.zw);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets4.xy);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets4.zw);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets5.xy);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets5.zw);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets6.xy);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets6.zw);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets7.xy);
	vSample += tex2D(_MainTex, i.uv+avSampleOffsets7.zw);
	
	vSample /= 16;
    
	return vSample;
}	

half4 SampleLumFinal (v2f_img i) : COLOR
{
    half fResampleSum = 0.0f;
    
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets0.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets0.zw);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets1.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets1.zw);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets2.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets2.zw);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets3.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets3.zw);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets4.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets4.zw);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets5.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets5.zw);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets6.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets6.zw);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets7.xy);
	fResampleSum += tex2D(_MainTex, i.uv+avSampleOffsets7.zw);	
    
    fResampleSum = exp(fResampleSum / 16);
    
    return half4(fResampleSum, fResampleSum, fResampleSum, 1.0f);
}

half4 CalculateAdaptedLum (v2f_img i) : COLOR
{
    half fAdaptedLum = tex2D(_MainTex, half2(0.5f, 0.5f));
    half fCurrentLum = tex2D(_SecondTex, half2(0.5f, 0.5f));

    half fNewAdaptation = fAdaptedLum + (fCurrentLum - fAdaptedLum) * ( 1 - pow( 0.98f, 30 * _ElapsedTime * _AdaptaionSpeed ) );
    
    fNewAdaptation = max(0.01f, fNewAdaptation);
    
    return half4(fNewAdaptation, fNewAdaptation, fNewAdaptation, 1.0f);
}

half4 FinalScenePass (v2f_img i) : COLOR
{
    half4 vSample = tex2D(_MainTex, i.uv);
	half4 vBloom = tex2D(_BloomTex, i.uv);

	half fAdaptedLum = tex2D(_SecondTex, half2(0.5f, 0.5f));
	
	vSample.rgb *= _MiddleGray/(fAdaptedLum + 0.001f);
	half3 x = max(fixed3(0,0,0),vSample.rgb-0.004);
	vSample.rgb=(x*(6.2*x+.5))/(x*(6.2*x+1.7)+0.06);

    vSample += vBloom*_BloomScale; 
    
	vSample.rgb = pow(vSample.rgb, 1/_GammaValue);  
	  
    return vSample;
}

half4 BrightPassFilter (v2f_img i) : COLOR
{
	half4 vSample = tex2D( _MainTex, i.uv );
	half  fAdaptedLum = tex2D( _SecondTex, half2(0.5f, 0.5f) );

	vSample.rgb *= _MiddleGray/(fAdaptedLum + 0.001f) * (1+vSample.rgb/(_MiddleGray*_MiddleGray));
	vSample.rgb -= _BrightPassThreshold;

	vSample = max(vSample, 0.0f);
	
	vSample.rgb /= (_BrightPassOffset+vSample);
    
	return vSample;
}

half4 GaussBlur5x5 (v2f_img i) : COLOR
{
	
    half4 vSample = 0.0f;

	vSample += avSampleWeights0 * tex2D( _MainTex, i.uv + avSampleOffsets0.xy );
	vSample += avSampleWeights1 * tex2D( _MainTex, i.uv + avSampleOffsets0.zw );
	vSample += avSampleWeights2 * tex2D( _MainTex, i.uv + avSampleOffsets1.xy );
	vSample += avSampleWeights3 * tex2D( _MainTex, i.uv + avSampleOffsets1.zw );
	vSample += avSampleWeights4 * tex2D( _MainTex, i.uv + avSampleOffsets2.xy );
	vSample += avSampleWeights5 * tex2D( _MainTex, i.uv + avSampleOffsets2.zw );
	vSample += avSampleWeights6 * tex2D( _MainTex, i.uv + avSampleOffsets3.xy );
	vSample += avSampleWeights7 * tex2D( _MainTex, i.uv + avSampleOffsets3.zw );
	vSample += avSampleWeights8 * tex2D( _MainTex, i.uv + avSampleOffsets4.xy );
	vSample += avSampleWeights9 * tex2D( _MainTex, i.uv + avSampleOffsets4.zw );
	vSample += avSampleWeights10 * tex2D( _MainTex, i.uv + avSampleOffsets5.xy );
	vSample += avSampleWeights11 * tex2D( _MainTex, i.uv + avSampleOffsets5.zw );

	return vSample;
}

half4 DownScale2x2 (v2f_img i) : COLOR
{
	
    half4 vSample = 0.0f;

	vSample += tex2D( _MainTex, i.uv + avSampleOffsets0.xy );
	vSample += tex2D( _MainTex, i.uv + avSampleOffsets0.zw );
	vSample += tex2D( _MainTex, i.uv + avSampleOffsets1.xy );
	vSample += tex2D( _MainTex, i.uv + avSampleOffsets1.zw );	
	   
	return vSample / 4;
}

half4 BloomBlurVertical (v2f_img i) : COLOR
{   
   half4 color = 0.0f;
 
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y - 4.0*_BloomBlurSizeV)) * 0.05;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y - 3.0*_BloomBlurSizeV)) * 0.09;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y - 2.0*_BloomBlurSizeV)) * 0.12;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y - _BloomBlurSizeV)) * 0.15;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y)) * 0.16;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y + _BloomBlurSizeV)) * 0.15;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y + 2.0*_BloomBlurSizeV)) * 0.12;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y + 3.0*_BloomBlurSizeV)) * 0.09;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y + 4.0*_BloomBlurSizeV)) * 0.05;
 
   return color;
}

half4 BloomBlurHorizontal (v2f_img i) : COLOR
{   
   half4 color = 0.0f;

   color += tex2D(_MainTex, half2(i.uv.x - 4.0*_BloomBlurSizeH, i.uv.y)) * 0.05;
   color += tex2D(_MainTex, half2(i.uv.x - 3.0*_BloomBlurSizeH, i.uv.y)) * 0.09;
   color += tex2D(_MainTex, half2(i.uv.x - 2.0*_BloomBlurSizeH, i.uv.y)) * 0.12;
   color += tex2D(_MainTex, half2(i.uv.x - _BloomBlurSizeH, i.uv.y)) * 0.15;
   color += tex2D(_MainTex, half2(i.uv.x, i.uv.y)) * 0.16;
   color += tex2D(_MainTex, half2(i.uv.x + _BloomBlurSizeH, i.uv.y)) * 0.15;
   color += tex2D(_MainTex, half2(i.uv.x + 2.0*_BloomBlurSizeH, i.uv.y)) * 0.12;
   color += tex2D(_MainTex, half2(i.uv.x + 3.0*_BloomBlurSizeH, i.uv.y)) * 0.09;
   color += tex2D(_MainTex, half2(i.uv.x + 4.0*_BloomBlurSizeH, i.uv.y)) * 0.05;
 
   return color;
}

half4 GammaCorrection (v2f_img i) : COLOR
{   
   half4 color = tex2D(_MainTex, i.uv);
   
   color.rgb = pow(max(half3(0.0f,0.0f,0.0f), color.rgb + _Offset), _GammaValue);

   return color;
}

half4 SetColorToSwap (v2f_img i) : COLOR
{
   return 0.5f;
}

	ENDCG
			
//-----------------------------------------------------------------------------
// Techniques
//-----------------------------------------------------------------------------
Subshader { 
//-----------------------------------------------------------------------------
// Name: SampleLumInitial   
// Pass: 0                               
//-----------------------------------------------------------------------------
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment SampleLumInitial
      ENDCG
  }

//-----------------------------------------------------------------------------
// Name: SampleLumIterative   
// Pass: 1                               
//-----------------------------------------------------------------------------
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment SampleLumIterative
      ENDCG
  }

//-----------------------------------------------------------------------------
// Name: DownScale4x4   
// Pass: 2                               
//-----------------------------------------------------------------------------
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment DownScale4x4
      ENDCG
  }
  
//-----------------------------------------------------------------------------
// Name: SampleLumFinal   
// Pass: 3                               
//-----------------------------------------------------------------------------
 Pass {
	  ZTest Always Cull Off ZWrite Off 
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment SampleLumFinal
      ENDCG
  } 
   
//-----------------------------------------------------------------------------
// Name: CalculateAdaptedLum   
// Pass: 4                               
//-----------------------------------------------------------------------------
 Pass {
	  ZTest Always Cull Off ZWrite Off 
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert_img
      #pragma fragment CalculateAdaptedLum
      ENDCG
  }  
   
//-----------------------------------------------------------------------------
// Name: FinalScenePass   
// Pass: 5                               
//-----------------------------------------------------------------------------
 Pass {
	  ZTest Always Cull Off ZWrite Off 
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment FinalScenePass
      ENDCG
  }     
  
//-----------------------------------------------------------------------------
// Name: BrightPassFilter   
// Pass: 6                               
//-----------------------------------------------------------------------------
 Pass {
	  ZTest Always Cull Off ZWrite Off 
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment BrightPassFilter
      ENDCG
  }  

//-----------------------------------------------------------------------------
// Name: GaussBlur5x5   
// Pass: 7                               
//-----------------------------------------------------------------------------
 Pass {
	  ZTest Always Cull Off ZWrite Off 
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 2.0
      #pragma vertex vert_img
      #pragma fragment GaussBlur5x5
      ENDCG
  }    
  
//-----------------------------------------------------------------------------
// Name: DownScale2x2   
// Pass: 8                               
//-----------------------------------------------------------------------------
 Pass {
	  ZTest Always Cull Off ZWrite Off 
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 2.0
      #pragma vertex vert_img
      #pragma fragment DownScale2x2
      ENDCG
  } 
    
//-----------------------------------------------------------------------------
// Name: BloomBlurVertical  
// Pass: 9                           
//-----------------------------------------------------------------------------
 Pass {
	  ZTest Always Cull Off ZWrite Off 
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment BloomBlurVertical
      ENDCG
  } 
  
//-----------------------------------------------------------------------------
// Name: BloomBlurHorizontal  
// Pass: 10                           
//-----------------------------------------------------------------------------
 Pass {
	  ZTest Always Cull Off ZWrite Off 
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment BloomBlurHorizontal
      ENDCG
  } 
//-----------------------------------------------------------------------------
// Name: GammaCorrection 
// Pass: 11                           
//-----------------------------------------------------------------------------
 Pass {
	  ZTest Always Cull Off ZWrite Off 
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment GammaCorrection
      ENDCG
  }      

//-----------------------------------------------------------------------------
// Name: SetColorToSwap 
// Pass: 12                           
//-----------------------------------------------------------------------------
 Pass {
	  ZTest Always Cull Off ZWrite Off 
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment SetColorToSwap
      ENDCG
  }      
}

Fallback off
}