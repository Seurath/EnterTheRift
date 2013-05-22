using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcSpawnControl : MonoBehaviour {
	
	public Transform[] spawnPoints;
	public int maxSimultaneous = 3;
	public int maxTotal = 0;
	public float spawnDelay = 1.0f;
	
	public Transform[] npcTypes;
	
	private bool[] usedSpawnPoints;
	private float spawnTimer = 0.0f;
	private List<Transform> activeNpcs = new List<Transform>();
	private int totalSpawned = 0;
	
	// Use this for initialization
	void Start () {
		if(spawnPoints.Length == 0)
		{
			Debug.Log ("No spawn points specified");
			enabled = false;
			return;
		}
		
		if(npcTypes.Length == 0)
		{
			Debug.Log ("No npc types specified.");
			enabled = false;
			return;
		}
		
		usedSpawnPoints = new bool[spawnPoints.Length];
		spawnTimer = spawnDelay;
	}
	
	void SpawnNpc(int spawnPointIndex)
	{
		int npcIndex = (int)Mathf.Round(Random.Range(0, npcTypes.Length));
		
		activeNpcs.Add((Transform)Instantiate(npcTypes[npcIndex], spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation));
		
		usedSpawnPoints[spawnPointIndex] = true;
		spawnTimer = spawnDelay;
		
		// Beh... double loop?
		bool atLeastOneUnused = false;
		foreach(bool isUsed in usedSpawnPoints)
		{
			if(!isUsed)
			{
				atLeastOneUnused = true;
			}
		}

		if(!atLeastOneUnused)
		{
			// We've used all the spawn points... so enable them all again
			for(int i = 0; i < usedSpawnPoints.Length; ++i)
			{
				usedSpawnPoints[i] = false;
			}
		}
		
		if(maxTotal > 0)
		{
			++totalSpawned;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(maxTotal > 0 && totalSpawned >= maxTotal)
		{
			// Stop updating this spawn control
			enabled = false;
			return;
		}
		
		activeNpcs.Remove(null);
		
		if(activeNpcs.Count < maxSimultaneous)
		{
			spawnTimer -= Time.deltaTime;
			
			if(spawnTimer < 0.0f)
			{
				List<int> unusedSpawnPoints = new List<int>();
				for(int i = 0; i < usedSpawnPoints.Length; ++i)
				{
					if(!usedSpawnPoints[i])
					{
						unusedSpawnPoints.Add(i);
					}
				}
				if(unusedSpawnPoints.Count > 0)
				{
					int spawnIndex = unusedSpawnPoints[(int)Mathf.Round(Random.Range(0, unusedSpawnPoints.Count))];
					SpawnNpc (spawnIndex);
				}
			}
		}
	}
}
