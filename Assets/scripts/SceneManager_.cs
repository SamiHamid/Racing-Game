using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneManager_ : MonoBehaviour
{
    public WaypointsContainer[] waypointsContainer;
    public SpawnpointContainer[] spawnpointContainer;
    public UpgradeMenu upgrade_menu;
    public Standings standings;
    public static SceneManager_ instance;
    public static ProgressTracker progress_tracker_instance;
    public GameObject race_manager;
    public bool isTrackLoaded = false;

    private ScreenFadeOut fadeOutLeft;
    private ScreenFadeOut fadeOutRight;
    private ScreenFadeIn fadeInLeft;
    private ScreenFadeIn fadeInRight;

    void Awake()
    {
        instance = this;
        progress_tracker_instance = ProgressTracker.instance;
        race_manager = GameObject.Find("RaceManager");
        fadeInLeft = GameObject.Find("Main Camera Left").GetComponent<ScreenFadeIn>();
        fadeInRight = GameObject.Find("Main Camera Right").GetComponent<ScreenFadeIn>();
        fadeOutLeft = GameObject.Find("Main Camera Left").GetComponent<ScreenFadeOut>();
        fadeOutRight = GameObject.Find("Main Camera Right").GetComponent<ScreenFadeOut>();
    }

    // Use this for initialization
    void Start()
    {
        fadeInLeft.enabled = true;
        fadeInRight.enabled = true;
        fadeOutLeft.enabled = false;
        fadeOutRight.enabled = false;
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

    public IEnumerator LoadNextTrack()
    {

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            fadeOutRight.enabled = true;
            fadeOutLeft.enabled = true;
            yield return new WaitForSeconds(5.0f);
            SceneManager.LoadScene("BridgeTrackScene");
        }
        if (SceneManager.GetActiveScene().name == "BridgeTrackScene")
        {
            fadeOutRight.enabled = true;
            fadeOutLeft.enabled = true;
            yield return new WaitForSeconds(5.0f);
            SceneManager.LoadScene("BlasterTrackScene");
        }
        
        isTrackLoaded = true;
    }

    // Update is called once per frame
    void Update()
    {
        #region Enter start race button on the main menu (will be implemented later)
        /* if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick2Button7) || Input.GetKeyDown(KeyCode.Joystick1Button1)){
           }
        */
        #endregion

        if (standings.isRewardSequenceFinished)
        {
            StartCoroutine(ViewUpgradeMenu());
        }

        if (isTrackLoaded)
        {
            StopAllCoroutines();
            upgrade_menu.gameObject.SetActive(false);
        }
    }

    IEnumerator ViewUpgradeMenu()
    {
        yield return new WaitForSeconds(3f);
        upgrade_menu.gameObject.SetActive(true);
    }
}

