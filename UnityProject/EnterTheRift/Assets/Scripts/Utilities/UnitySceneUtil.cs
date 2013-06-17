using System;
using UnityEngine;

public static class UnitySceneUtility
{
	public static GameObject AnchorGameObjectAtTransform (GameObject target, Transform transform)
	{
		return AnchorGameObjectAtTransform(target, transform, true);
	}
	
	public static GameObject AnchorGameObjectAtTransform (GameObject target, Transform transform, bool doNormalizeScale)
	{
		target.transform.parent = transform;
		target.transform.localPosition = Vector3.zero;
		if (doNormalizeScale) {
			target.transform.localScale = Vector3.one;
		}
		return target;
	}
	
	public static GameObject InstantiatePrefabAtTransform (UnityEngine.Object prefab, Transform transform)
	{
		GameObject instance = GameObject.Instantiate(prefab) as GameObject;
		if (instance == null)
		{ 
			Debug.LogError("Provided prefab object cannot be instantiated to a GameObject type.");
		}
		return AnchorGameObjectAtTransform(instance, transform);
	}
	
	public static GameObject InstantiateGameObjectAtTransform (GameObject target, Transform transform)
	{
		GameObject instance = GameObject.Instantiate(target) as GameObject;
		return AnchorGameObjectAtTransform(instance, transform);
	}
	
	public static GameObject InstantiateGameObjectAtTransform (string name, Transform parentTransform)
	{
		GameObject gameObject = new GameObject(name);
		return AnchorGameObjectAtTransform(gameObject, parentTransform);
	}
	
	public static void DestroyChildren (Transform parentTransform)
	{
		for (int i = 0; i < parentTransform.GetChildCount(); i++) 
		{
			GameObject.Destroy(parentTransform.GetChild(i).gameObject);
		}
	}
	
}
