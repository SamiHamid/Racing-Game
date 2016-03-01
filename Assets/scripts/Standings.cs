using UnityEngine;
using System.Collections;

public class Standings : MonoBehaviour
{
    public static Standings instance;

    public GameObject stage1;
    public GameObject stage2;
    public GameObject stage3;

    public float speed = 0.05f;

    private float startTime;
    private float journeyLength1;
    private float journeyLength2;
    private float journeyLength3;

    public GameObject endMarker1;
    public GameObject endMarker2;
    public GameObject endMarker3;

    public bool isRewardSequenceFinished = false;
    public int playerRank;
    public int numberOfRewarded = 0;
    public static RaceManager race_manager_instance;

    public void Awake()
    {
        instance = this;
        stage1 = GameObject.Find("Stage1");
        stage2 = GameObject.Find("Stage2");
        stage3 = GameObject.Find("Stage3");

        endMarker1 = GameObject.Find("endMarker1");
        endMarker2 = GameObject.Find("endMarker2");
        endMarker3 = GameObject.Find("endMarker3");
    }

    void Start()
    {
        startTime = Time.time;
        race_manager_instance = RaceManager.instance;
    }

    void Update()
    {
        //Uncomment if you want to update race standings after all racers finish
        // if (RaceManager.instance.AllRacersFinished())
        StartCoroutine(UpdateRaceStandings());
       
    }


    public IEnumerator UpdateRaceStandings()
    {
        float distCovered = (Time.time - startTime) * speed;

        for (int i = 0; i < RankManager.instance.totalRacers; i++)
        {
            if (RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Statistics>().lap > GetComponent<RaceManager>().totalLaps)
            {
                

                if (RankManager.instance.racerRanks[i].racer.GetComponent<Statistics>().rank == 1)
                {
                    journeyLength1 = Vector3.Distance(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker1.transform.position);
                    float fracJourney1 = distCovered / journeyLength1;

                    RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>().controllable = false;
                    RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<ProgressTracker>().enabled = false;
                    yield return new WaitForSeconds(1);
                    RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    yield return new WaitForSeconds(1);
                    RankManager.instance.racerRanks[i].racer.gameObject.transform.position = Vector3.Lerp(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker1.transform.position, fracJourney1);

                    numberOfRewarded = 1;

                    if (race_manager_instance.playerCar)
                        playerRank = 1;
                }

                if (RankManager.instance.racerRanks[i].racer.GetComponent<Statistics>().rank == 2)
                {
                    journeyLength2 = Vector3.Distance(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker2.transform.position);
                    float fracJourney2 = distCovered / journeyLength2;


                    // if (RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Statistics>().lap > GetComponent<RaceManager>().totalLaps)
                    {
                        RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>().controllable = false;
                        RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<ProgressTracker>().enabled = false;
                        yield return new WaitForSeconds(1);
                        RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        yield return new WaitForSeconds(1);
                        RankManager.instance.racerRanks[i].racer.gameObject.transform.position = Vector3.Lerp(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker2.transform.position, fracJourney2);

                        numberOfRewarded = 2;

                        if (race_manager_instance.playerCar)
                            playerRank = 2;
                    }
                }
                if (RankManager.instance.racerRanks[i].racer.GetComponent<Statistics>().rank == 3)
                {
                    journeyLength3 = Vector3.Distance(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker3.transform.position);
                    float fracJourney3 = distCovered / journeyLength3;

                    // if (RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Statistics>().lap > GetComponent<RaceManager>().totalLaps)
                    {
                        RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>().controllable = false;
                        RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<ProgressTracker>().enabled = false;
                        yield return new WaitForSeconds(1);
                        RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        yield return new WaitForSeconds(1);
                        RankManager.instance.racerRanks[i].racer.gameObject.transform.position = Vector3.Lerp(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker3.transform.position, fracJourney3);

                        numberOfRewarded = 3;

                        if (race_manager_instance.playerCar)
                            playerRank = 3;
                    }
                }

                if (RankManager.instance.racerRanks[i].racer.GetComponent<Statistics>().rank == 4)
                {
                    if (race_manager_instance.raceCompleted)
                    {
                        RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>().controllable = false;

                        RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<ProgressTracker>().enabled = false;

                        // yield return new WaitForSeconds(0);

                        RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Rigidbody>().isKinematic = true;

                        if (race_manager_instance.playerCar)
                            playerRank = 4;
                    }
                }
            }
        }
        

        //If race is completed and all cars are rewarded, reward sequence is finished. 
        if(race_manager_instance.raceCompleted && numberOfRewarded < RaceManager.instance.totalRacers)
        {
            isRewardSequenceFinished = true;
        }
    }
}

