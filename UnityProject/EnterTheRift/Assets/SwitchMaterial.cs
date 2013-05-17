using UnityEngine;
using System.Collections;

public class SwitchMaterial : MonoBehaviour {
	
	public Material switchMaterial;
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void DoMaterialSwitch()
	{
		if(switchMaterial != null)
		{
			renderer.material = switchMaterial;
		}
	}
	
}
