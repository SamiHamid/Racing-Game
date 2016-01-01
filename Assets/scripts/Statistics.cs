//Statistics.cs keeps track of the racer's rank, lap, race times, race state, saving best times, split times , wrong way detecion etc.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Statistics : MonoBehaviour
{
    public static Statistics instance;

    //Int
    public int rank;//current rank
	public int lap; //current lap
	
	
	//Strings
	public string currentLapTime; //current lap time string displayed by RaceUI.cs
	public string prevLapTime; //Previous lap time string displayed by RaceUI.cs
	public string totalRaceTime; //Total lap time string displayed by RaceUI.cs
	
	//Floats
	private float lapTimeCounter; // keeps track of our current Lap time counter
	private float totalTimeCounter; //keeps track of our total race time
	private float currentBestTime; //keeps track of the current session best time in TimeTrial
	private float dotProduct; //used for wrong way detection
	private float registerDistance = 10.0f; //distance to register a passed node
	private float reviveTimer;
	
	//Hidden Vars
	[HideInInspector]public Transform lastPassedNode; //last node to passed - used when respawning.
	[HideInInspector]public Transform target; //progress tracker target
	[HideInInspector]public int currentNodeNumber; //next node index in the "path" list
	[HideInInspector]public List <Transform> path = new List<Transform>();
	[HideInInspector]public List <bool> passednodes = new List<bool>();
	[HideInInspector]public List <Transform> checkpoints = new List<Transform>();
	[HideInInspector]public List <bool> passedcheckpoints = new List<bool>();
	[HideInInspector]public bool finishedRace;
	[HideInInspector]public bool knockedOut;
	[HideInInspector]public bool goingWrongway;
	[HideInInspector]public bool passedAllNodes;
	[HideInInspector]public float speedRecord;//speed trap top speed
    

    void OnEnable () {
		//Disable components of no race manager is found.
		if(!RaceManager.instance){
			this.enabled = false;
			GetComponent<ProgressTracker>().enabled = this.enabled;
		}
		else{
			FindPath();
			//Initialize();
		}
	}
	
	void Initialize()
    {
        lap = 1;
	}

    void Awake()
    {
        //create an instance
        instance = this;
        //rank_manager = GetComponent<RankManager>();
    }

    void Start()
    {
        Initialize();
    }
	
	
	void FindPath(){
		Transform pathContainer = RaceManager.instance.pathContainer;
		Transform[] nodes = pathContainer.GetComponentsInChildren<Transform>();
		
		foreach(Transform p in nodes){
			
			if(p != pathContainer){
				path.Add(p);
			}
		}
		passednodes = new List <bool>(new bool[path.Count]);
		lastPassedNode = path[0];
	}
	
	void Update () {
		GetPath();
		CalculateLapTime();
		CalculateAngleDifference();
		Revive();
	}
	
	
	void GetPath(){
		int n = currentNodeNumber;
		
		Transform node = path[n] as Transform;
		Vector3 nodeVector = target.InverseTransformPoint(node.position);
		
		//register that we have passed this node
		if (nodeVector.magnitude <= registerDistance){
			currentNodeNumber++;
			passednodes[n] = true;
			
			//set our last passed node
			if(n != 0)
				lastPassedNode = path[n - 1];
			else
				lastPassedNode = path[path.Count - 1];
		}
		
		//Check if all nodes have been passed
		foreach(bool pass in passednodes){
			if(pass == true){
				passedAllNodes = true;
			}
			else{
				passedAllNodes = false;
			}
		}
		
		//Reset the currentNodeNumber after passing all the nodes
		if(currentNodeNumber >= path.Count){  
			currentNodeNumber = 0; 
		}
	}
	
	
	// Race time calculations
	void CalculateLapTime(){
		
		if(RaceManager.instance.raceStarted && !knockedOut && !finishedRace)
        { 
            lapTimeCounter += Time.deltaTime;	
			totalTimeCounter += Time.deltaTime;
		}
		
		//Format the time strings
		currentLapTime = RaceManager.instance.FormatTime(lapTimeCounter);
		totalRaceTime = RaceManager.instance.FormatTime(totalTimeCounter);
	}
	
	//Called on new lap
	public void NewLap(){
		
		/*if(gameObject.tag =="Player"){
			CheckForBestTime();
		}*/
		
		//Reset our passed nodes & checkpoints
		/*for(int i = 0; i < passednodes.Count; i++){
			passednodes[i] = false;
		}
	
			if(lap < RaceManager.instance.totalLaps){
				lap++; 
			}*/
			/*else{
				if(!finishedRace){
					FinishRace();
				}
			}*/
		
        		
		//Set the previous lap time and reset the lap counter
		prevLapTime = currentLapTime;
	}
	
	/*void CheckForBestTime(){
		
		//Save a new best time if we dont currently have one
		if(PlayerPrefs.GetFloat("BestTimeFloat"+Application.loadedLevelName) == 0){
			PlayerPrefs.SetString("BestTime"+Application.loadedLevelName,currentLapTime);
			PlayerPrefs.SetFloat("BestTimeFloat"+Application.loadedLevelName,lapTimeCounter);
			PlayerPrefs.Save();
		}
		//Save a new best time if we beat our current best time
		if(PlayerPrefs.GetFloat("BestTimeFloat"+Application.loadedLevelName) > lapTimeCounter){
			PlayerPrefs.SetString("BestTime"+Application.loadedLevelName,currentLapTime);
			PlayerPrefs.SetFloat("BestTimeFloat"+Application.loadedLevelName,lapTimeCounter);
			PlayerPrefs.Save();
		}
	}*/

    
	
	/*void FinishRace()
    {
	    	
		//Tell the RaceManager that player has finished the race
		//if(gameObject.tag == "Player"){
			RaceManager.instance.EndRace(rank);
		//}

       // GetComponent<CarController>().controllable = false;
	
		finishedRace = true;	
	}*/
	
	
	// Switches a player car to an AI controlled car
	public void AIMode(){
		if(GetComponent<PlayerController>()){
			Destroy(GetComponent<PlayerController>());
			gameObject.AddComponent<SteeringBehaviours>();
		}
	}
	
	
	// Switches a AI car to an human controlled car
	public void PlayerMode(){
		if(GetComponent<PlayerController>()){
			Destroy(GetComponent<SteeringBehaviours>());
			if(!GetComponent<PlayerController>()){
				gameObject.AddComponent<PlayerController>();
			}
			else{
				GetComponent<PlayerController>().enabled = true;
			}
		}
	}
	
	// Check for wrong way
	void CalculateAngleDifference(){
		float nodeAngle = target.transform.eulerAngles.y;
		float transformAngle = transform.eulerAngles.y;
		float angleDifference = nodeAngle - transformAngle;
		
		if (Mathf.Abs(angleDifference) <= 230f && Mathf.Abs(angleDifference) >= 120){
			if(GetComponent<Rigidbody>().velocity.magnitude * 2.237f > 10.0f){
				goingWrongway = true;
			}
			else{
				goingWrongway = false;
			}
		}
		else{
			goingWrongway = false;
		}
	}
	
	void Revive()
    {
		//incase the car flips over or going wrong way then respawn
		/*if(transform.localEulerAngles.z > 80 && transform.localEulerAngles.z < 280 || RaceManager.instance.forceWrongwayRespawn && goingWrongway){
			reviveTimer += Time.deltaTime;
		}
		else{*/
			reviveTimer = 0.0f;
		//} 
		
		if(reviveTimer >= 5.0f){
			RaceManager.instance.RespawnRacer(transform,lastPassedNode);
			reviveTimer = 0.0f;
		}	
	}
	
	void OnTriggerEnter(Collider other){
		//Finish line
		if(other.tag == "LapPoint" && passedAllNodes){
            //NewLap();
            for (int i = 0; i < passednodes.Count; i++)
            {
                passednodes[i] = false;
            }

            lap++;

           /* if (lap > RaceManager.instance.totalLaps)
            {
                GetComponent<CarController>().controllable = false;
            }*/


        }
	}
}
