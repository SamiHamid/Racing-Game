using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneManager_ : MonoBehaviour
{
    public GameObject[] tracksContainer; //10 tracks
    public WaypointsContainer[] waypointsContainer;
    public SpawnpointContainer[] spawnpointContainer;
    public UpgradeMenu upgrade_menu;
    public Standings standings;
    public Application[] scenes;
    private int track_index;
   // public Transform[] scenes;

    public static SceneManager_ instance;
    public static ProgressTracker progress_tracker_instance;
    public GameObject race_manager;

    //public bool isTrackSelected = false;
    public bool isTrackLoaded = false;


    void Awake()
    {
        instance = this;
        progress_tracker_instance = ProgressTracker.instance;
        race_manager = GameObject.Find("RaceManager");
    }

    // Use this for initialization
    void Start()
    {
        track_index = 0;
        // InitializeRaceManager();
        InitializeTrack();
        
    }

    void InitializeRaceManager()
    {
        //RaceManager.instance.gameObject.SetActive(true);
        InitializeTrack();
    }

    public void InitializeTrack()
    {
       // tracksContainer[track_index].SetActive(true);
       // RaceManager.instance.pathContainer = waypointsContainer[track_index].transform;
       // RaceManager.instance.spawnpointContainer = spawnpointContainer[track_index].transform;
    }

    //Player can choose track (this will be enabled if it is decided to go with it) 
    /*void ChooseTrack()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            tracksContainer[1].SetActive(false);
            tracksContainer[2].SetActive(false);
            tracksContainer[0].SetActive(true);
            race_manager.pathContainer = waypointsContainer[0].transform;
            race_manager.spawnpointContainer = spawnpointContainer[0].transform;
            isTrackSelected = true;

        }
        else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            tracksContainer[0].SetActive(false);
            tracksContainer[2].SetActive(false);
            tracksContainer[1].SetActive(true);
            race_manager.pathContainer = waypointsContainer[1].transform;
            race_manager.spawnpointContainer = spawnpointContainer[1].transform;
            isTrackSelected = true;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            tracksContainer[0].SetActive(false);
            tracksContainer[1].SetActive(false);
            tracksContainer[2].SetActive(true);
            race_manager.pathContainer = waypointsContainer[2].transform;
            race_manager.spawnpointContainer = spawnpointContainer[2].transform;
            isTrackSelected = true;
        }
    }*/

    public int GetTrackIndex()
    {
        return track_index;
    }

    public void LoadNextTrack()
    {
        
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            SceneManager.LoadScene("BridgeTrackScene");
        }
        if (SceneManager.GetActiveScene().name == "BridgeTrackScene")
        {
            SceneManager.LoadScene("BlasterTrackScene");
        }
        /* track_index++;

         for (int i = 0; i < tracksContainer.Length; i++)
         {
             if (i == track_index)
             {
                 RaceManager.instance.pathContainer = waypointsContainer[i].transform;
                 RaceManager.instance.spawnpointContainer = spawnpointContainer[i].transform;
                 tracksContainer[i].SetActive(true);
                 continue;
             }
             else
                 tracksContainer[i].SetActive(false);
         }*/

        isTrackLoaded = true;
        //standings.Reset();
        //RankManager.instance.Reset();


        //Destroy(RaceManager.instance.GetComponent<RaceManager>());
        //RaceManager.instance.gameObject.AddComponent<RaceManager>();
        //Destroy(RaceManager.instance.GetComponent<RankManager>());
        //RaceManager.instance.gameObject.AddComponent<RankManager>();
        //Destroy(RaceManager.instance.GetComponent<Standings>());
        //RaceManager.instance.gameObject.AddComponent<Standings>();

        /*for (int i = 0; i < RankManager.instance.totalRacers; i++)
        {
            Destroy(RankManager.instance.racerRanks[i].racer.GetComponent<Statistics>());
            RankManager.instance.racerRanks[i].racer.AddComponent<Statistics>();
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        //Enter start race button on the main menu (will be implemented later)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick2Button7) || Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            InitializeRaceManager();
        }

        if (standings.isRewardSequenceFinished)
        {
            StartCoroutine(ViewUpgradeMenu());
            
        }

        if (isTrackLoaded)
        {
            //standings.Reset();
            StopAllCoroutines();

            upgrade_menu.gameObject.SetActive(false);
        }
    }

    IEnumerator ViewUpgradeMenu()
    {
        yield return new WaitForSeconds(3f);
        upgrade_menu.gameObject.SetActive(true);

       
        //RaceManager.instance.opponentCars.Clear();
        //RaceManager.instance.playerCar = null;
        
    }

}

