using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    public GameObject[] tracksContainer; //10 tracks
    public WaypointsContainer[] waypointsContainer;
    public SpawnpointContainer[] spawnpointContainer;
    public UpgradeMenu upgrade_menu;
    public Standings standings;

    private int track_index;

    public static SceneManager instance;
    public RaceManager race_manager;

    //public bool isTrackSelected = false;
    public bool enableRaceManager = false;
    public bool isTrackLoaded = false;


    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        track_index = 0;
       // InitializeRaceManager();
    }

    void InitializeRaceManager()
    {
        race_manager.gameObject.SetActive(true);
        InitializeTrack();
    }

    void InitializeTrack()
    {
        tracksContainer[track_index].SetActive(true);
        race_manager.pathContainer = waypointsContainer[track_index].transform;
        race_manager.spawnpointContainer = spawnpointContainer[track_index].transform;
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
        track_index++;
      
        for (int i = 0; i < tracksContainer.Length; i++)
        {
            if (i == track_index)
            {
                race_manager.pathContainer = waypointsContainer[i].transform;
                race_manager.spawnpointContainer = spawnpointContainer[i].transform;
                tracksContainer[i].SetActive(true);
                continue;
            }
            else
                tracksContainer[i].SetActive(false);
        }

        isTrackLoaded = true;

        race_manager.Reset();
        // standings.Reset();
        RankManager.instance.Reset();
        upgrade_menu.gameObject.SetActive(false);

    }

	// Update is called once per frame
	void Update ()   
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

        if(isTrackLoaded)
        {
            standings.Reset();
            StopAllCoroutines();
           
           

        }
    }

    IEnumerator ViewUpgradeMenu()
    {
        yield return new WaitForSeconds(3f);
        upgrade_menu.gameObject.SetActive(true);
        
        standings.isRewardSequenceFinished = false;

        
    }

}

