using UnityEngine;
using System.Collections;

public class CubemapFace : MonoBehaviour {
	
	public 	Material	matCube;
	public	Material	matQuad;
	
	public 	bool	renderQuad = false;
	public	bool	renderBlurQuad = false;
	
	public	Transform	cube;
	
	public void Create()	{
		GameObject go = new GameObject();
		go.name = "cubemap_" + go.GetInstanceID().ToString();
		go.transform.parent = transform;
		go.transform.localPosition = new Vector3(0, 0, 2.207f);
		go.transform.localRotation = Quaternion.identity;
		go.hideFlags = HideFlags.HideAndDontSave; 
		go.layer = gameObject.layer;
		 
		cube = go.transform;
		
		MeshFilter mf = go.AddComponent<MeshFilter>();
		MeshRenderer mr = go.AddComponent<MeshRenderer>();
		Mesh mesh = mf.mesh;
   		mesh.vertices = new Vector3[] {	new Vector3(-0.5f, -0.5f, 0.5f), 	new Vector3(-0.5f, -0.5f, -0.5f), 
										new Vector3(0.5f, -0.5f, -0.5f),	new Vector3(0.5f, -0.5f, 0.5f), 
										new Vector3(-0.5f, 0.5f, 0.5f), 	new Vector3(0.5f, 0.5f, 0.5f),
										new Vector3(0.5f, 0.5f, -0.5f), 		new Vector3(-0.5f, 0.5f, -0.5f)		};
        mesh.triangles = new int[] {0,1,2, 2,3,0, 4,5,6, 6,7,4, 0,3,5, 5,4,0, 3,2,6, 6,5,3, 2,1,7, 7,6,2, 1,0,4, 4,7,1};
		mesh.normals = new Vector3[] {	new Vector3(-0.5774f, -0.5774f, 0.5774f), 	new Vector3(-0.5774f, -0.5774f, -0.5774f), 
										new Vector3(0.5774f, -0.5774f, -0.5774f),	new Vector3(0.5774f, -0.5774f, 0.5774f), 
										new Vector3(-0.5774f, 0.5774f, 0.5774f), 	new Vector3(0.5774f, 0.5774f, 0.5774f), 
										new Vector3(0.5774f, 0.5774f, -0.5774f), 	new Vector3(-0.5774f, 0.5774f, -0.5774f) };	
		mf.mesh = mesh;
		mr.material = matCube;
		mr.castShadows = false;
		mr.receiveShadows = false;
	}
	
	public float offset = 0.0f;
	
	void OnPostRender(){
		float o = 0;
		if (renderQuad)	{
			GL.PushMatrix();
			if (renderBlurQuad)	{ matQuad.SetPass(1); o = offset;}
			else matQuad.SetPass(0);		
			GL.Color(new Color(1,1,1,1));
			GL.LoadOrtho();
			GL.Begin(GL.QUADS);
			GL.TexCoord(new Vector3(o,o,0));
			GL.Vertex3(0,0,0);
			GL.TexCoord(new Vector3(o,1-o,0));
			GL.Vertex3(0,1,0);
			GL.TexCoord(new Vector3(1-o,1-o,0));
			GL.Vertex3(1,1,0);
			GL.TexCoord(new Vector3(1-o,o,0));
			GL.Vertex3(1,0,0);
			GL.End();
			GL.PopMatrix();	 
		}
	}
	
//	void OnDestroy()	{
////		DestroyImmediate(matQuad);
////		DestroyImmediate(matCube);		
////		DestroyImmediate(cube.gameObject);
//	}
}
