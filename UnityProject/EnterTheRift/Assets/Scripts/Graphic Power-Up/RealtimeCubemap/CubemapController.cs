using UnityEngine;
using System.Collections;

[AddComponentMenu("Graphic Power-Up/Real-time Cubemap-reflections")]
public class CubemapController : MonoBehaviour {
	
	public	bool			stopUpdate = false;
	public	int				cubemapSize = 256;
	public	RefreshRate		refreshRate = RefreshRate.Medium;
	public	float			farClip = 100.0f;
	public	float			nearClip = 0.1f;
	public	RenderingPath	renderingPath;
	public	ColorSpace 		colorSpace = ColorSpace.Gamma;
	public	LayerMask		cullingMask = -1;
	public	bool			shadowed = false;
	
	public	Texture		blankCubemap;
	
	
	public	Shader	antialiasingShader;
	public	Shader	encodeShader;
	public	Shader	cubeShader;
	public	Shader	quadShader;
	public	Shader	blurShader;
	public	Shader	shadowShader;
			
	private	Material	aaMat;
	private	Material	encodeMat;
	private	Material	cubeMat;
	private	Material	quadMat;
	private	Material	blurMat;
	private	Material	shadowMat;
	
	private	RenderTexture	cubemapLdr;
	private	RenderTexture	cubemap;
	private	RenderTexture	cubemapBlur;
	
	private	RenderTexture	rtHdr;
	private	RenderTexture	rtBlurHdr;
	private	RenderTexture	rtEncode;
	private	RenderTexture	rtBlurEncode;
	
	private	Camera	cubemapCamera;
	private Camera	cubemapCameraEncode;
	private	CubemapFace	cubemapFace;
	
	private int faceToRender = 1;
	
//	private Mesh blobMesh;
	
	public enum RefreshRate
	{
		Slow = 1, Medium = 2, Fast = 4, VeryFast = 6	
	}
		
	void OnEnable()	{
		if (!cubeShader || !encodeShader || !quadShader)	{
			Debug.Log("Not all shaders");
			this.enabled = false;
			return;
		}
		if (SystemInfo.graphicsShaderLevel != 30)	{
			Debug.Log("Not supported Shader Model 3.0");
			this.enabled = false;
			return;		
		}
		if (!SystemInfo.supportsRenderTextures)	{
			Debug.Log("Not supported Render Textures");
			this.enabled = false;
			return;
		}
		if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))	{
			Debug.Log("Not supported ARGBHalf(Floating-point) Render Textures");
			this.enabled = false;
			return;
		}
		
		encodeMat = new Material(encodeShader);
		cubeMat = new Material(cubeShader);
		quadMat = new Material(quadShader);
		blurMat = new Material(blurShader);
		shadowMat = new Material(shadowShader);
		
		rtHdr = new RenderTexture(cubemapSize, cubemapSize, 24, RenderTextureFormat.ARGBHalf);
		rtEncode = new RenderTexture(cubemapSize, cubemapSize, 24, RenderTextureFormat.ARGB32);
		rtBlurHdr = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBHalf);
		rtBlurEncode = new RenderTexture(256, 256, 24, RenderTextureFormat.ARGB32);
		
		GameObject go = new GameObject();
		go.name = "cubemapCamera_" + go.GetInstanceID().ToString();
		go.hideFlags = HideFlags.HideAndDontSave;
		go.transform.parent = transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		
		cubemapCamera = go.AddComponent<Camera>();
		cubemapCamera.fieldOfView = 90.0f;
		cubemapCamera.aspect = 1.0f;
		cubemapCamera.backgroundColor = Color.black;
		cubemapCamera.hdr = true;
		cubemapCamera.targetTexture = rtHdr;
		cubemapCamera.enabled = false;
		
		
		go = new GameObject();
		go.name = "cubemapCameraRgbm_" + go.GetInstanceID().ToString();
		go.hideFlags = HideFlags.HideAndDontSave;
		go.transform.parent = transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.layer = gameObject.layer;
		
		
		cubemapCameraEncode = go.AddComponent<Camera>();
		cubemapCameraEncode.cullingMask = 1<<gameObject.layer;
		cubemapCameraEncode.clearFlags = CameraClearFlags.Nothing;
		cubemapCameraEncode.fieldOfView = 45.0f;
		cubemapCameraEncode.aspect = 1.0f;
		cubemapCameraEncode.renderingPath = RenderingPath.VertexLit;
		cubemapCameraEncode.enabled = false;
		cubemapCameraEncode.targetTexture = rtEncode;
		cubemapCameraEncode.depth = 24;
		
		cubemapFace = go.AddComponent<CubemapFace>();
		cubemapFace.matCube = cubeMat;
		cubemapFace.matQuad = quadMat;


		cubemapFace.Create();	
		
		cubemapLdr = new RenderTexture(cubemapSize, cubemapSize, 24);
		cubemapLdr.isCubemap = true;
		cubemapLdr.isPowerOfTwo = true;
		
		cubemap = new RenderTexture(cubemapSize, cubemapSize, 24);
		cubemap.isCubemap = true;
		cubemap.isPowerOfTwo = true;
		
		cubemapBlur = new RenderTexture(128, 128, 24);
		cubemapBlur.isCubemap = true;
		cubemapBlur.isPowerOfTwo = true;
		
		Shader.SetGlobalTexture("_Cube", cubemapLdr);
		Shader.SetGlobalTexture("_CubeHDR", cubemap);
		Shader.SetGlobalTexture("_CubeBlurHDR", cubemapBlur);
		
		if (antialiasingShader)
			aaMat = new Material(antialiasingShader);
				
		cubemapFace.matCube.SetTexture("_CubeStatic", cubemap);
	}
	
	
	void OnDisable() 
	{
		Shader.SetGlobalTexture("_Cube", blankCubemap);
		Shader.SetGlobalTexture("_CubeHDR", blankCubemap);
		Shader.SetGlobalTexture("_CubeBlurHDR", blankCubemap);
		
//		DestroyImmediate(blobMesh);
		
		DestroyImmediate(rtHdr);
		DestroyImmediate(rtBlurHdr);
		DestroyImmediate(rtEncode);
		DestroyImmediate(rtBlurEncode);
		
		DestroyImmediate(cubemapFace.cube.gameObject);
		DestroyImmediate(cubemapFace.matQuad);
		Destroy(cubemapCamera.gameObject);
		Destroy(cubemapCameraEncode.gameObject);
		
		DestroyImmediate(cubemapLdr);
		DestroyImmediate(cubemap);
		DestroyImmediate(cubemapBlur);
		
		DestroyImmediate(aaMat);
		DestroyImmediate(encodeMat);
		DestroyImmediate(cubeMat);
		DestroyImmediate(quadMat);
		DestroyImmediate(blurMat);
		DestroyImmediate(shadowMat);
	}
	
	Vector3 lastPos;
	int		encodeMatPass = 0;
	
	void Update()	{
			
		if ((colorSpace == ColorSpace.Gamma || colorSpace == ColorSpace.Uninitialized) && encodeMatPass == 1)	{
			encodeMatPass = 0;	
		}
		else if (colorSpace == ColorSpace.Linear && encodeMatPass == 0)	{
			encodeMatPass = 1;	
		}
				
		if (!stopUpdate)	{
			for (int i = 0; i < (int)refreshRate; i++)	{
				UpdateCubemap(faceToRender);
				faceToRender++;
				if (faceToRender > 5)	{
					faceToRender = 0;
				}
			}
		}
	}
	
	void UpdateCubemap(int face)	{
		Vector3 v = Vector3.zero;
		if 		(face == 1)	v = new Vector3(0,270,0);
		else if (face == 2)	v = new Vector3(270,0,0);
		else if (face == 3)	v = new Vector3(90,0,0);
		else if (face == 4)	v = new Vector3(0,0,0);
		else if (face == 5)	v = new Vector3(180,0,180);
		else if (face == 0)	v = new Vector3(0,90,0);
		
		if (cubemapCamera)	{
			cubemapCamera.farClipPlane = farClip;
			cubemapCamera.nearClipPlane = nearClip;
			cubemapCamera.cullingMask = cullingMask;
			cubemapCamera.renderingPath = renderingPath;
			cubemapCamera.transform.rotation = Quaternion.Euler(v);
			cubemapCamera.Render();
		}
		
		if (aaMat)
			Graphics.Blit(rtHdr, rtHdr, aaMat);
		if (face == 3 && shadowed && shadowMat)
			Graphics.Blit(rtHdr, rtHdr, shadowMat);
		
		
		cubemapFace.cube.localRotation = Quaternion.Euler(new Vector3(v.x,v.y,v.z+180));	
		if (face == 4 || face == 5)			cubemapFace.cube.localScale = new Vector3(1,1,2);
		else if (face == 1 || face == 0)	cubemapFace.cube.localScale = new Vector3(2,1,1);
		else if (face == 2 || face == 3)	cubemapFace.cube.localScale = new Vector3(1,2,1);
		else cubemapFace.cube.localScale = new Vector3(1,1,1);	
		 
		cubemapFace.renderQuad = true;
		cubemapFace.matQuad.SetTexture("_MainTex", rtHdr);
		cubemapCameraEncode.RenderToCubemap(cubemapLdr, 1 << face);
		cubemapFace.renderQuad = false;
			
		Graphics.Blit(rtHdr, rtEncode, encodeMat, encodeMatPass);
		cubemapFace.renderQuad = true;
		cubemapFace.matQuad.SetTexture("_MainTex", rtEncode);
		cubemapCameraEncode.RenderToCubemap(cubemap, 1 << face);
		cubemapFace.renderQuad = false;
		
		cubemapCameraEncode.targetTexture = rtBlurHdr;
		cubemapCameraEncode.Render();

		blurMat.SetFloat("blurSize", 1.66f / (float)rtBlurHdr.width);
		for(int i = 0; i < 2; i++)	{
			Graphics.Blit(rtBlurHdr, rtBlurHdr, blurMat, 0);
			Graphics.Blit(rtBlurHdr, rtBlurHdr, blurMat, 1);
		}
		Graphics.Blit(rtBlurHdr, rtBlurEncode, encodeMat, encodeMatPass);
		cubemapFace.matQuad.SetTexture("_MainTex", rtBlurEncode);
		cubemapFace.offset = 0.405f;
		cubemapFace.renderBlurQuad = true;
		cubemapFace.renderQuad = true;
		cubemapCameraEncode.RenderToCubemap(cubemapBlur, 1 << face);
		cubemapFace.renderQuad = false;
		cubemapFace.renderBlurQuad = false;

	}
	
}
