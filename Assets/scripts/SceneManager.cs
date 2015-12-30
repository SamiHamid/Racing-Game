using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    public GameObject[] tracksContainer; //10 tracks
    public WaypointsContainer[] waypointsContainer;
    public SpawnpointContainer[] spawnpointContainer;
    public RaceManager race_manager;
    public UpgradeMenu upgrade_menu;
    public RewardSequence reward_sequence;


    // Use this for initialization
    void Start()
    {
        
    }

    void InitializeRaceManager()
    {
        race_manager.gameObject.SetActive(true);
        for (int i = 0; i < tracksContainer.Length; i++)
        {
            waypointsContainer[i] = tracksContainer[i].GetComponentInChildren<WaypointsContainer>();
            spawnpointContainer[i] = tracksContainer[i].GetComponentInChildren<SpawnpointContainer>();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick2Button7) ||  Input.GetKeyDown(KeyCode.Joystick1Button1))
            InitializeRaceManager();

        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            tracksContainer[1].SetActive(false);
            tracksContainer[2].SetActive(false);
            tracksContainer[0].SetActive(true);
            race_manager.pathContainer = waypointsContainer[0].transform;
            race_manager.spawnpointContainer = spawnpointContainer[0].transform;

        }
        else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            tracksContainer[0].SetActive(false);
            tracksContainer[2].SetActive(false);
            tracksContainer[1].SetActive(true);
            race_manager.pathContainer = waypointsContainer[1].transform;
            race_manager.spawnpointContainer = spawnpointContainer[1].transform;

        }
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            tracksContainer[0].SetActive(false);
            tracksContainer[1].SetActive(false);
            tracksContainer[2].SetActive(true);
            race_manager.pathContainer = waypointsContainer[2].transform;
            race_manager.spawnpointContainer = spawnpointContainer[2].transform;
        }

        if (reward_sequence.isRewardSequenceFinished)
        {
            StartCoroutine(ViewUpgradeMenu());
        }


    }



    IEnumerator ViewUpgradeMenu()
    {
        yield return new WaitForSeconds(3f);
        upgrade_menu.gameObject.SetActive(true);
    }

}

