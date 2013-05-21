using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Graphic Power-Up/Post-Processing")]
[RequireComponent (typeof(Camera))]
public class PostProcessing : MonoBehaviour {
	
	public float keyValue = 1.0f;			// Middle gray key value for tone mapping
	public float adaptaionSpeed = 1.0f;
	public float gammaValue = 2.2f;
	public float offsetValue = 0.002f;
	public float bloomScale = 1.0f;
	public float bloomSize = 1.0f;
	public float bloomThreshold = 1.0f;
	
	public Shader shader;
	private Material material;

//	public Shader colorCorrectionShader;
//	private Material colorCorrectionMaterial;
	
	private RenderTexture sourceScaled = null;				// Scaled copy of the HDR scene
	private RenderTexture brightPass = null;				// Bright-pass filtered copy of the scene
	private RenderTexture adaptedLuminanceCur = null;		// The luminance that the user is currenly adapted to
	private RenderTexture adaptedLuminanceLast = null;		// The luminance that the user is currenly adapted to	
	private RenderTexture swap = null;	
	private RenderTexture bloomSource = null;				// Bloom effect source texture	
	
	private RenderTexture[] bloom = new RenderTexture[3];
	private RenderTexture[] toneMap = new RenderTexture[4];	
	
	const int samples = 16;
	
	void SourceToSourceScaled(RenderTexture source)
	{
		DownSample4x(source, sourceScaled);		
	}
	
	void SourceScaledToBrightPass()
	{
		material.SetTexture("_SecondTex", adaptedLuminanceCur);
		Graphics.Blit(sourceScaled, brightPass, material, 6);		
	}
	
	void BrightPassToBloomSource()
	{
		Vector2[] avSampleOffsets = new Vector2[samples];
		Vector4[] avSampleWeights = new Vector4[samples];
		
		GetSampleOffsets_GaussBlur5x5(brightPass.width, brightPass.height,
		                              avSampleOffsets, avSampleWeights, 1.0f);	
		
		SetSampleOffsets(avSampleOffsets);
		SetSampleWeights(avSampleWeights);
		Graphics.Blit(brightPass, brightPass, material, 7);
		
		
		GetSampleOffsets_DownScale2x2(brightPass.width, 
		                              brightPass.height, avSampleOffsets);
		
		SetSampleOffsets(avSampleOffsets);
		
		Graphics.Blit(brightPass, bloomSource, material, 8);	
	}
	
	void RenderBloom()
	{
		Vector2[] avSampleOffsets = new Vector2[samples];
		Vector4[] avSampleWeights = new Vector4[samples];
		
		GetSampleOffsets_GaussBlur5x5( bloomSource.width, bloomSource.height, 
		                              avSampleOffsets, avSampleWeights, 1.0f );
		
		SetSampleOffsets(avSampleOffsets);
		SetSampleWeights(avSampleWeights);
		
		Graphics.Blit(bloomSource, bloom[2], material, 7);
		
		Graphics.Blit(bloom[2], bloom[1], material, 9);
		Graphics.Blit(bloom[1], bloom[0], material, 10);
		
//		GetSampleOffsets_Bloom( bloom[2].width, afSampleOffsets, 
//		                       avSampleWeights, 3.0f, 2.0f );
//		
//		for( i = 0; i < samples; i++ )
//		{
//			avSampleOffsets[i] = new Vector2( afSampleOffsets[i], 0.0f );
//		}
//		
//		SetSampleOffsets(avSampleOffsets);
//		SetSampleWeights(avSampleWeights);
//		
//		Graphics.Blit(bloom[2], bloom[1], HDRMaterial, 9);
//		
//		for( i = 0; i < samples; i++ )
//		{
//			avSampleOffsets[i] = new Vector2(0.0f, afSampleOffsets[i]);
//		}
//		
//		SetSampleOffsets(avSampleOffsets);
//		SetSampleWeights(avSampleWeights);
//		
//		Graphics.Blit(bloom[1], bloom[0], HDRMaterial, 9);
		
	}
	
	void MeasureLuminance()
	{
		int x,y,index;
		Vector2[] avSampleOffsets = new Vector2[samples];
		
		int curTexture = toneMap.Length - 1;
		
		float tU, tV;
		tU = 1.0f / (3.0f * toneMap[curTexture].width);
		tV = 1.0f / (3.0f * toneMap[curTexture].height);
		
		index = 0;
		for (x = -1; x <= 1; x++)
		{
			for (y = -1; y <= 1; y++)
			{
				avSampleOffsets[index].x = x * tU;
				avSampleOffsets[index].y = y * tV;
				
				index++;
			}
		}
		
		SetSampleOffsets(avSampleOffsets);					
		
		Graphics.Blit(sourceScaled, toneMap[curTexture], material, 1);
		
		curTexture--;
		
		while( curTexture > 0 )
    	{	
			avSampleOffsets = GetSampleOffsets_DownScale4x4(toneMap[curTexture].width, 
		                                                toneMap[curTexture].height);		
			SetSampleOffsets(avSampleOffsets);	
			Graphics.Blit(toneMap[curTexture+1], toneMap[curTexture], material, 2);
			curTexture--;	
		}
		
		avSampleOffsets = GetSampleOffsets_DownScale4x4(toneMap[1].width, 
		                                                toneMap[1].height);		
		
		SetSampleOffsets(avSampleOffsets);
		
		Graphics.Blit(toneMap[1], toneMap[0], material, 3);		
	}
	
	void CalculateAdaptation()
	{	
		swap = adaptedLuminanceLast;
		adaptedLuminanceLast = adaptedLuminanceCur;
		adaptedLuminanceCur = swap;
		
		material.SetTexture("_SecondTex", toneMap[0]);
		material.SetFloat("_ElapsedTime", Time.deltaTime);
		Graphics.Blit(adaptedLuminanceLast, adaptedLuminanceCur, material, 4);		
	}
	
	Vector2[] GetSampleOffsets_DownScale4x4( int dwWidth, int dwHeight)
	{
    	float tU = 1.0f / (float)dwWidth;
    	float tV = 1.0f / (float)dwHeight;
		Vector2[] avSampleOffsets = new Vector2[samples];

	    int index = 0;
	    for( int y = 0; y < 4; y++ )
	    {
	        for( int x = 0; x < 4; x++ )
	        {
	            avSampleOffsets[ index ].x = ( (float)x - 1.5f ) * tU;
	            avSampleOffsets[ index ].y = ( (float)y - 1.5f ) * tV;
	
	            index++;
        	}
   		}

    	return avSampleOffsets;
	}
	
	void SetSampleOffsets (Vector2[] avSampleOffsets)
	{
		for (int i = 0; i < samples / 2; i++)
		{
				int j = i*2;
				material.SetVector("avSampleOffsets"+SwitchIntToString(i), 
			    	new Vector4(avSampleOffsets[j].x, avSampleOffsets[j].y, avSampleOffsets[j+1].x, avSampleOffsets[j+1].y));	
		}
	}
	
	void SetSampleWeights (Vector4[] avSampleWeights)
	{
		for (int i = 0; i < samples; i++)
				material.SetVector("avSampleWeights"+SwitchIntToString(i),  avSampleWeights[i]);	
	}
			
	string SwitchIntToString(int i)
	{
		switch (i)
		{
			case 0:	return "0";
			case 1:	return "1";
			case 2:	return "2";
			case 3:	return "3";
			case 4:	return "4";
			case 5: return "5";
			case 6:	return "6";
			case 7:	return "7";
			case 8:	return "8";
			case 9:	return "9";
			case 10:	return "10";
			case 11:	return "11";
			case 12:	return "12";
			case 13:	return "13";
			case 14:	return "14";
			case 15:	return "15";
		}
		return "";
	}
	
	private void DownSample4x (RenderTexture source, RenderTexture dest)
	{		
		
		Vector2[] avSampleOffsets = GetSampleOffsets_DownScale4x4(source.width, source.height);
		
		SetSampleOffsets(avSampleOffsets);
		Graphics.Blit(source, dest, material, 2);
	}
	
	void GetSampleOffsets_GaussBlur5x5(int dwD3DTexWidth, int dwD3DTexHeight, 
	                                   Vector2[] avTexCoordOffset,
	                                   Vector4[] avSampleWeight,
	                                   float fMultiplier)
	{
    	float tu = 1.0f / ( float )dwD3DTexWidth;
    	float tv = 1.0f / ( float )dwD3DTexHeight;	
		
		Vector4 vWhite = Vector4.one;
		
		float totalWeight = 0.0f;
		int index = 0;
		for (int x = -2; x <= 2; x++)
		{
			for (int y = -2; y <= 2; y++)
			{
				if ( (Mathf.Abs(x) + Mathf.Abs(y)) > 2)	
					continue;
				avTexCoordOffset[index] =  new Vector2(x * tu, y * tv);
				avSampleWeight[index] = vWhite * GaussianDistribution( ( float )x, ( float )y, 1.0f );
				totalWeight += avSampleWeight[index].x;
				
				index++;
			}
		}
		
		for( int i = 0; i < index; i++ )
	    {
	        avSampleWeight[i] /= totalWeight;
	        avSampleWeight[i] *= fMultiplier;
	    }
	}
	
	private float GaussianDistribution( float x, float y, float rho )
	{
	    float g = 1.0f / Mathf.Sqrt( 2.0f * Mathf.PI * rho * rho );
	    g *= Mathf.Exp( -( x * x + y * y ) / ( 2 * rho * rho ) );
	
	    return g;
	}
	
	private void GetSampleOffsets_DownScale2x2(int dwWidth, int dwHeight, 
	                                      Vector2[] avSampleOffsets)
	{
	    float tU = 1.0f / dwWidth;
	    float tV = 1.0f / dwHeight;
	
	    int index = 0;
	    for( int y = 0; y < 2; y++ )
	    {
	        for( int x = 0; x < 2; x++ )
	        {
	            avSampleOffsets[ index ].x = ( x - 0.5f ) * tU;
	            avSampleOffsets[ index ].y = ( y - 0.5f ) * tV;
	
	            index++;
	        }
	    }
	}
	
	void OnEnable()	{
//		if (!isCreated)	{
//			Create(new Vector2(1024, 768));
//		}	
	}
	
//	private bool isCreated = false;
	
	void Create(Vector2 size) 
	{	
		camera.hdr = true;
		
		material = new Material(shader);

		int width = (int)size.x;
		int height = (int)size.y;
		
		sourceScaled = new RenderTexture(width/4, height/4, 0, RenderTextureFormat.ARGBHalf);	
		brightPass = new RenderTexture(width/4 + 2, height/4 + 2, 0, RenderTextureFormat.ARGBHalf);
		bloomSource = new RenderTexture(width/8 + 2, height/8 + 2, 0, RenderTextureFormat.ARGBHalf);		
		adaptedLuminanceCur = new RenderTexture(1, 1, 0, RenderTextureFormat.ARGBHalf);
		adaptedLuminanceLast = new RenderTexture(1, 1, 0, RenderTextureFormat.ARGBHalf);		
		swap = new RenderTexture(1, 1, 0, RenderTextureFormat.ARGBHalf);
		toneMap[0] = new RenderTexture(1, 1, 0, RenderTextureFormat.ARGBHalf);
		toneMap[1] = new RenderTexture(4, 4, 0, RenderTextureFormat.ARGBHalf);
		toneMap[2] = new RenderTexture(16, 16, 0, RenderTextureFormat.ARGBHalf);
		toneMap[3] = new RenderTexture(64, 64, 0, RenderTextureFormat.ARGBHalf);		
		bloom[0] = new RenderTexture(width/8 + 2, height/8 + 2, 0, RenderTextureFormat.ARGBHalf);
		bloom[1] = new RenderTexture(width/8 + 2, height/8 + 2, 0, RenderTextureFormat.ARGBHalf);
		bloom[2] = new RenderTexture(width/8 + 2, height/8 + 2, 0, RenderTextureFormat.ARGBHalf);	
				
		Graphics.Blit(null, swap, material, 12);
		
//		isCreated = true;
	}
		
	void OnDisable() 
	{
//		isCreated = false;
		
		DestroyImmediate(material);
		
		DestroyImmediate(bloomSource);			
		DestroyImmediate(sourceScaled);
		DestroyImmediate(adaptedLuminanceCur);
		DestroyImmediate(adaptedLuminanceLast);
		DestroyImmediate(swap);
		for (int i = 0; i < toneMap.Length; i++)
			DestroyImmediate(toneMap[i]);
		for (int i = 0; i < bloom.Length; i++)
			DestroyImmediate(bloom[i]);
		DestroyImmediate(brightPass);
	}
	
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		if (sourceScaled == null)	{
			Create(new Vector2(source.width, source.height));
		}
		
		if (material != null) 
		{
			Graphics.Blit(source, source, material, 11);
		
		
	        SourceToSourceScaled(source);		
	        MeasureLuminance();     
	        CalculateAdaptation();
			
			SourceScaledToBrightPass();
			BrightPassToBloomSource();
			RenderBloom();
			
			material.SetTexture("_SecondTex", adaptedLuminanceCur);
			material.SetFloat("_MiddleGray", keyValue);
			material.SetFloat("_AdaptaionSpeed", adaptaionSpeed);	
			material.SetFloat("_GammaValue", gammaValue);
			material.SetFloat("_Offset", offsetValue);
			material.SetTexture("_BloomSourceTex", bloomSource);
			material.SetTexture("_BloomTex", bloom[0]);
			material.SetFloat("_BloomScale", bloomScale);
			material.SetFloat("_BloomBlurSizeV", bloomSize/source.height);
			material.SetFloat("_BloomBlurSizeH", bloomSize/source.width);
			material.SetFloat("_BrightPassThreshold", bloomThreshold);
			material.SetFloat("_BrightPassOffset", bloomThreshold*2.0f);
				
			Graphics.Blit(source, destination, material, 5);
		}
	}
	
	
}
