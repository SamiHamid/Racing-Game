//spawnpoint container.cs helps you visually place your race spawnpoints.
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SpawnpointContainer : MonoBehaviour {
	
	[HideInInspector]
	public Transform[] spawnpoints;
	public Color spawnpointColor = new Color(1,0,0,.5f);
	
	void OnDrawGizmos() {
		Gizmos.color = spawnpointColor;
		if(spawnpoints.Length > 0){
			for(int i = 1; i < spawnpoints.Length; i++){
				Gizmos.DrawSphere (spawnpoints[i].position,.5f);
			}
		}
	}
	
	void Update () {
		Transform[] transforms = GetComponentsInChildren<Transform>();
		
		spawnpoints = new Transform[transforms.Length];
		
		for(int i = 0; i < spawnpoints.Length; i++){
			spawnpoints[i] = transforms[i];
		}
		
		int c = 1;
		foreach(Transform child in transforms){
			if(child != transform){
				child.name = (c++).ToString("00");
			}
		}
	}
}
