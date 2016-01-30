/*Race_Manager.cs handles the race logic - countdown, spawning cars, asigning racer names, checking race status, formatting time strings etc */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO; 

public class RaceManager : MonoBehaviour {
	
	public static RaceManager instance;
    
    public int totalLaps = 1;
	public int totalRacers = 4; //The total number of racers (player included)
	public int playerStartRank = 2; //The rank you will start the race as
	public float raceDistance; //Your race track's distance.
    [HideInInspector]public float countdownDelay = 3.0f;
	public GameObject playerCar;
	public List <GameObject> opponentCars = new List <GameObject>();
   
	public Transform pathContainer;
	public Transform spawnpointContainer;
	[HideInInspector]public List<Transform> spawnpoints = new List <Transform>();
	//public string playerName = "You";
	//public List<string> opponentNamesList = new List<string>();
	//public TextAsset opponentNames;
	//public StringReader nameReader;
	//public GameObject playerPointer, opponentPointer, racerName;
	//public bool showRacerNames = true; //Should names appear above player cars
	//public bool showRacerPointers = true; //Should minimap pointers appear above all racers
	//public bool showRaceInfoMessages = true;//Show final lap indication , new best lap, speed trap & racer knockout information texts
	//public bool forceWrongwayRespawn; //should the player get respawned if going the wrong way
	public bool raceStarted = false; //has the race began
	public bool raceCompleted = false; //have the all cars finished the race
	public bool racePaused = false; //is the game paused

	//Rewards
	public List<RaceRewards> raceRewards = new List<RaceRewards>();
	
	void Awake () {
		//create an instance
		instance = this;
        
    }

    void Start()
    {
        Debug.Log("Started");

        Object[] AICars = Resources.LoadAll("AI Cars", typeof(GameObject));
        playerCar = (GameObject) Resources.Load("PlayerCars/Car2_Player");

        foreach(GameObject AICar in AICars)
        {
            opponentCars.Add(AICar);
        }

        SceneManager.instance.InitializeTrack();
        InitializeRace();
	}
	
	void InitializeRace(){
		
		ConfigureNodes();
        SpawnRacers();
        StartCountdown();
	}
	
	void SpawnRacers(){
		
		if(!playerCar){
			Debug.LogError("Please add a player car!");
			return;
		}
		
		spawnpoints.Clear();
        
    
		//Find the children of the spawnpoint container and add them to the spawnpoints List.
		Transform[] _sp = spawnpointContainer.GetComponentsInChildren<Transform>();
		foreach(Transform point in _sp){
			if(point != spawnpointContainer){
				spawnpoints.Add(point);
			}
		}
		
		
		//limit the total amount of race cars according to the spawnpoint count
		if(totalRacers > spawnpoints.Count){
			totalRacers = spawnpoints.Count;
		}
		else if (totalRacers <= 0){
			totalRacers = 1;
		}
		
		//Make sure the player spawns at a reasonable rank if "playerStartRank" is configured in an incorrect manner
		if(playerStartRank > spawnpoints.Count){
			playerStartRank = spawnpoints.Count;
		}
		else if(playerStartRank > totalRacers){
			playerStartRank = totalRacers;
		}
		else if(playerStartRank <= 0){
			playerStartRank = 1;
		}
		
		//spawn the cars
		for(int i = 0; i < totalRacers; i++){
			if(spawnpoints[i] != spawnpoints[playerStartRank-1] && opponentCars.Count > 0){
				Instantiate(opponentCars[Random.Range(0,opponentCars.Count)],spawnpoints[i].position,spawnpoints[i].rotation);

            }
			else if(spawnpoints[i] == spawnpoints[playerStartRank-1] && playerCar){
				
					Instantiate(playerCar,spawnpoints[i].position,spawnpoints[i].rotation);
            }
        }
		
		//Set racer names, pointers and begin countdown after spawning the racers
		RankManager.instance.RefreshRacerCount();
		
	}


    public void No3()
    {
       Countdown.instance.countdownArray[0].SetActive(true);
       Countdown.instance.countdownArray[1].SetActive(false);
        
    }

    public void No2()
    {
        Countdown.instance.countdownArray[0].SetActive(false);
        Countdown.instance.countdownArray[1].SetActive(true);
 
    }

    public void No1()
    {
        Countdown.instance.countdownArray[2].SetActive(true);
        Countdown.instance.countdownArray[1].SetActive(false);
        
    }

    public void Go()
    {
       Countdown.instance.countdownArray[0].SetActive(false);
       Countdown.instance.countdownArray[1].SetActive(false);
       Countdown.instance.countdownArray[2].SetActive(false);
       Countdown.instance.countdownArray[3].SetActive(true);

    }


    private void DisableGo()
    {
        Countdown.instance.countdownArray[3].SetActive(false);
    }

    public void StartCountdown()
    {
        Invoke("StartRace", countdownDelay + 2);
        Invoke("No3", 1f);
        Invoke("No2", 2f);
        Invoke("No1", 3f);
        Invoke("Go", 4f);
        Invoke("DisableGo", 5f);
    }





    void Update()
    {
		//Pause the race with "Escape".
		if(!raceCompleted && Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick2Button7) || Input.GetKeyDown(KeyCode.Joystick1Button9))
        {
			PauseRace();
		}

        if (AllRacersFinished() == true)
            EndRace();
	}
	
	public void StartRace () {
		//enable cars to start racing
		CarController[] cars = GameObject.FindObjectsOfType(typeof(CarController)) as CarController[];
		foreach(CarController c in cars){
			c.controllable = true;
		}
		raceStarted = true;
	}

    public void UpdateBudget()
    {
        if(Standings.instance.playerRank == 1)
        {
            PlayerData.AddCurrency(150);
            Debug.Log("Budget: " + PlayerData.currency);
        }
        
        if(Standings.instance.playerRank == 2)
        {
            PlayerData.AddCurrency(100);
            Debug.Log("Budget: " + PlayerData.currency);
        }
        if(Standings.instance.playerRank == 3)
        {
            PlayerData.AddCurrency(50);
            Debug.Log("Budget: " + PlayerData.currency);
        }
    }

    public void EndRace()
    {
       // if (AllRacersFinished() == true)
        {
            raceCompleted = true;

            //UpdateBudget();

            //update UI panels
            //RaceUI.instance.HandlePanelActivation();

            //Debug.Log("You finished " + rank + " in " + _raceType + " race");

            //Race Rewards
           /* if (raceRewards.Count >= rank)
            {
                Debug.Log("Race Rewards - " + "Currency : " + raceRewards[rank - 1].currency + " Car Unlock : " + raceRewards[rank - 1].carUnlock + " Track Unlock : " + raceRewards[rank - 1].trackUnlock);
                //give currency
                PlayerData.AddCurrency(raceRewards[rank - 1].currency);
                //unlock car
                if (raceRewards[rank - 1].carUnlock != "")
                    PlayerData.Unlock(raceRewards[rank - 1].carUnlock);
                //unlock track
                if (raceRewards[rank - 1].trackUnlock != "")
                    PlayerData.Unlock(raceRewards[rank - 1].trackUnlock);

                //sets reward text in RaceUI
                // RaceUI.instance.SetRewardText(raceRewards[rank - 1].currency.ToString("N0"), raceRewards[rank - 1].carUnlock, raceRewards[rank - 1].trackUnlock);
            }*/
        }
	}
	
	
	public void PauseRace(){
		racePaused = !racePaused;
		
		//update UI panels
		//RaceUI.instance.HandlePanelActivation();
		
		//Freeze the game & minimize volume on pause
		if(racePaused){
			Time.timeScale = 0.0f;
			AudioListener.volume = 0.1f;
		}
		else{
			Time.timeScale = 1.0f;
			AudioListener.volume = 1.0f;
		}
	}
	
	//Format a float to a time string
	public string FormatTime(float time){
		int minutes  = (int)Mathf.Floor(time / 60);
		int seconds = (int)time % 60;
		int milliseconds = (int)(time * 100) % 100;
		
		return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
	}


    // Checks if all racers have finished
    public bool AllRacersFinished()
    {
        int finished = 0;

        bool allFinished = false;
        Statistics[] allRacers = GameObject.FindObjectsOfType(typeof(Statistics)) as Statistics[];
        for (int i = 0; i < allRacers.Length; i++)
        { 

            if (allRacers[i].lap > totalLaps)
            {
                finished++;
            }

            if (finished == allRacers.Length)
            {
                allFinished = true;
            }
            else
            {
                allFinished = false;
            }  
        }

        return allFinished;
    }
	
	//Used to calculate track distance(in Meters) & rotate the nodes correctly
	void ConfigureNodes(){
		Transform[] m_path = pathContainer.GetComponentsInChildren<Transform>();
		List<Transform> m_pathList = new List<Transform>();
		foreach(Transform node in m_path){
			if( node != pathContainer){
				m_pathList.Add(node);
			}
		}
		for(int i = 0; i < m_pathList.Count; i++){
			if(i < m_pathList.Count-1){
				m_pathList[i].transform.LookAt(m_pathList[i+1].transform);
				raceDistance += Vector3.Distance(m_pathList[i].position,m_pathList[i + 1].position);
			}
			else{
				m_pathList[i].transform.LookAt(m_pathList[0].transform);
			}
		}
	}
	
	//used to respawn a racer
	public void RespawnRacer(Transform racer, Transform node){
        if(raceCompleted == false)
		Respawn(racer,node);
	}
	
	public void Respawn(Transform racer, Transform node){
		//Flip the car over and place it at the last passed node
		racer.rotation = Quaternion.LookRotation(racer.forward);
		racer.GetComponent<Rigidbody>().velocity = Vector3.zero;
		racer.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		racer.position = new Vector3(node.position.x, node.position.y + 2.0f, node.position.z);
		racer.rotation = node.rotation;
	}
}