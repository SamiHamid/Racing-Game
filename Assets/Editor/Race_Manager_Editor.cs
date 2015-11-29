using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(RaceManager))]
public class Race_Manager_Editor : Editor {
	
	RaceManager m_target;
	
	
	public void OnEnable () {
		m_target = (RaceManager)target;
	}
	
	public override void OnInspectorGUI(){
		//RACE SETTINGS
		GUILayout.BeginVertical("Box");
		GUILayout.Box("Race Settings",EditorStyles.boldLabel);
		EditorGUILayout.Space();
	
		
		EditorGUILayout.Space();
		
		
			m_target.totalLaps = EditorGUILayout.IntField("Total Laps",m_target.totalLaps);
		
		
		
			m_target.totalRacers = EditorGUILayout.IntField("Total Racers",m_target.totalRacers);
		
		
		
		

		GUILayout.EndVertical();
		
		EditorGUILayout.Space();
		
		//RACE CONTAINER SETTINGS
		GUILayout.BeginVertical("Box");
		GUILayout.Box("Race Container Settings",EditorStyles.boldLabel);
		EditorGUILayout.Space();

       

        //Path
        if (!m_target.pathContainer){
			
			if(!GameObject.FindObjectOfType(typeof(WaypointsContainer))){
				EditorGUILayout.HelpBox("Create a Path!",MessageType.Warning);
			}
			else{
				EditorGUILayout.HelpBox("Assign the Path!",MessageType.Info);
			}
			
			EditorGUILayout.Space();
			if(!GameObject.FindObjectOfType(typeof(WaypointsContainer))){
				if(GUILayout.Button("Create Path",GUILayout.Width(190))){
					//RGSK_Editor.CreatePath();
				}
			}
			else{
				if(GUILayout.Button("Assign Path",GUILayout.Width(190))){
					WaypointsContainer path = GameObject.FindObjectOfType(typeof(WaypointsContainer)) as WaypointsContainer;
					m_target.pathContainer = path.GetComponent<Transform>();
				}
			}
		}
		EditorGUILayout.Space();
		
		m_target.pathContainer = EditorGUILayout.ObjectField("Path Container",m_target.pathContainer,typeof(Transform),true) as Transform;
		
		//Spawnpoint
		if(!m_target.spawnpointContainer){
			
			if(!GameObject.FindObjectOfType(typeof(SpawnpointContainer))){
				EditorGUILayout.HelpBox("Create a Spawnpoint Container!",MessageType.Warning);
			}
			else{
				EditorGUILayout.HelpBox("Assign the Spawnpoint Container!",MessageType.Info);
			}
			
			EditorGUILayout.Space();
			
			if(!GameObject.FindObjectOfType(typeof(SpawnpointContainer))){
				if(GUILayout.Button("Create Spawnpoint Container",GUILayout.Width(190))){
					//RGSK_Editor.CreateSpawnpoint();
				}
			}
			else{
				if(GUILayout.Button("Assign Spawnpoint Container",GUILayout.Width(190))){
					SpawnpointContainer sp = GameObject.FindObjectOfType(typeof(SpawnpointContainer)) as SpawnpointContainer;
					m_target.spawnpointContainer = sp.GetComponent<Transform>();
				}
			}
		}
		
		m_target.spawnpointContainer = EditorGUILayout.ObjectField("Spawnpoint Container",m_target.spawnpointContainer,typeof(Transform),true) as Transform;  
		
		//Checkpoint
		/*if(!m_target.checkpointContainer){
			if(!GameObject.FindObjectOfType(typeof(CheckpointContainer))){
					EditorGUILayout.HelpBox("Speed Trap races require checkpoints. You can create a Checkpoint Container using the button below",MessageType.Info);
			}
			else{
				EditorGUILayout.HelpBox("Assign the Checkpoint Container!",MessageType.Info);
			}
			
			EditorGUILayout.Space();
			
			if(!GameObject.FindObjectOfType(typeof(CheckpointContainer))){
				if(GUILayout.Button("Create Checkpoint Container",GUILayout.Width(190))){
					RGSK_Editor.CreateCheckpoint();
				}
			}
			else{
				if(GUILayout.Button("Assign Checkpoint Container",GUILayout.Width(190))){
					CheckpointContainer cp = GameObject.FindObjectOfType(typeof(CheckpointContainer)) as CheckpointContainer;
					m_target.checkpointContainer = cp.GetComponent<Transform>();
				}
			}
		}
		m_target.checkpointContainer = EditorGUILayout.ObjectField("Checkpoint Container",m_target.checkpointContainer,typeof(Transform),true) as Transform;*/
		
		GUILayout.EndVertical();
		
		EditorGUILayout.Space();
		
		//RACE CAR SETTINGS
		GUILayout.BeginVertical("Box");
		GUILayout.Box("Race Car Settings",EditorStyles.boldLabel);
		EditorGUILayout.Space();
		m_target.playerCar = EditorGUILayout.ObjectField("Player Car Prefab:",m_target.playerCar,typeof(GameObject),true) as GameObject;
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		GUILayout.Label("Opponent Car Prefabs :");
		EditorGUILayout.Space();
		for(int i = 0; i < m_target.opponentCars.Count; i++){
			m_target.opponentCars[i] = EditorGUILayout.ObjectField((i+1).ToString(),m_target.opponentCars[i],typeof(GameObject),true) as GameObject;
		}
		EditorGUILayout.Space();
		if(GUILayout.Button("Add Opponent",GUILayout.Width(130))){
			GameObject newOpponent = null;
			m_target.opponentCars.Add(newOpponent);
		}
		if(GUILayout.Button("Remove Opponent",GUILayout.Width(130))){
			if(m_target.opponentCars.Count > 0){
				m_target.opponentCars.Remove(m_target.opponentCars[m_target.opponentCars.Count - 1]);
			}
		}
		GUILayout.EndVertical();
		
		EditorGUILayout.Space();
		
		//SPAWN SETTINGS
		GUILayout.BeginVertical("Box");
		GUILayout.Box("Spawn Settings",EditorStyles.boldLabel);
		EditorGUILayout.Space();
		m_target.playerStartRank = EditorGUILayout.IntField("Player Start Rank",m_target.playerStartRank);
		GUILayout.EndVertical();
		
		EditorGUILayout.Space();
		
		//MISC SETTINGS
		GUILayout.BeginVertical("Box");
		GUILayout.Box("Misc Settings",EditorStyles.boldLabel);
		//EditorGUILayout.Space();
		
		m_target.countdownDelay = EditorGUILayout.FloatField("Countdown Delay",m_target.countdownDelay);
        m_target.Number1 = EditorGUILayout.ObjectField("Number 1 Prefab:", m_target.Number1, typeof(GameObject), true) as GameObject;
        m_target.Number2 = EditorGUILayout.ObjectField("Number 2 Prefab:", m_target.Number2, typeof(GameObject), true) as GameObject;
        m_target.Number3 = EditorGUILayout.ObjectField("Number 3 Prefab:", m_target.Number3, typeof(GameObject), true) as GameObject;
        m_target.GoObject = EditorGUILayout.ObjectField("Go Prefab:", m_target.GoObject, typeof(GameObject), true) as GameObject;
        EditorGUI.BeginDisabledGroup (true);
		m_target.raceStarted = EditorGUILayout.Toggle("Race Started",m_target.raceStarted);
		m_target.raceCompleted = EditorGUILayout.Toggle("Race Completed",m_target.raceCompleted);
		m_target.racePaused = EditorGUILayout.Toggle("Race Paused",m_target.racePaused);
		EditorGUI.EndDisabledGroup ();
		GUILayout.EndVertical();
		
		EditorGUILayout.Space();
		
		
			
			EditorGUILayout.Space();
		}
		
		
		
		/*GUILayout.Label("Opponent Names :");
	for(int i = 0; i < m_target.opponentNamesList.Count; i++){
	EditorGUI.BeginDisabledGroup (true);
	m_target.opponentNamesList[i] = EditorGUILayout.TextField("", m_target.opponentNamesList[i]);
	EditorGUI.EndDisabledGroup ();
	}*/
		
		
	
}
