using UnityEngine;
using System.Collections;

public class RewardSequence : MonoBehaviour
{

    public GameObject stage1;
    public GameObject stage2;
    public GameObject stage3;

    public float speed = 0.05f;

    private float startTime;
    private float journeyLength1;
    private float journeyLength2;
    private float journeyLength3;

    public Transform endMarker1;
    public Transform endMarker2;
    public Transform endMarker3;

    // Use this for initialization
    void Start ()
    {
        startTime = Time.time;
       
    }
	
	// Update is called once per frame
	void Update ()
    {
       // if (RaceManager.instance.AllRacersFinished())
            StartCoroutine(UpdateRaceStandings()); 
	}


    /*void FinishRace()
    {
        
    }*/




    IEnumerator UpdateRaceStandings()
    {
        float distCovered = (Time.time - startTime) * speed;

        
        for (int i = 0; i < RankManager.instance.totalRacers; i++)
        {

            if (RankManager.instance.racerRanks[i].racer.GetComponent<Statistics>().rank == 1 )
            {
                journeyLength1 = Vector3.Distance(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker1.position);
                float fracJourney1 = distCovered / journeyLength1;

      
                if (RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Statistics>().lap > GetComponent<RaceManager>().totalLaps)
                { 
                    RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>().controllable = false;

                    yield return new WaitForSeconds(2);
                    RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<ProgressTracker>().enabled = false;
                    RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    //RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>().enabled = false;
                    //RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<SteeringBehaviours>().enabled = false;

                    RankManager.instance.racerRanks[i].racer.gameObject.transform.position = Vector3.Lerp(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker1.position, fracJourney1);
                }
            }

            else if (RankManager.instance.racerRanks[i].racer.GetComponent<Statistics>().rank == 2 )
            {
                journeyLength2 = Vector3.Distance(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker2.position);
                float fracJourney2 = distCovered / journeyLength2;


                if (RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Statistics>().lap > GetComponent<RaceManager>().totalLaps )
                {
                    RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>().controllable = false;

                    yield return new WaitForSeconds(2);
                    RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<ProgressTracker>().enabled = false;
                    RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    //RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>().enabled = false;
                    //RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<SteeringBehaviours>().enabled = false;

                    Debug.Log("Current Lap of Second Car: " + RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Statistics>().lap);

                    RankManager.instance.racerRanks[i].racer.gameObject.transform.position = Vector3.Lerp(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker2.position, fracJourney2);
                    
                   
                }

            }
            else if (RankManager.instance.racerRanks[i].racer.GetComponent<Statistics>().rank == 3 )
            {
                journeyLength3 = Vector3.Distance(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker3.position);
                float fracJourney3 = distCovered / journeyLength3;


                if (RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Statistics>().lap > GetComponent<RaceManager>().totalLaps)
                {
                    RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>().controllable = false;
                    RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<ProgressTracker>().enabled = false;
                    //RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>().enabled = false;
                    //RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<SteeringBehaviours>().enabled = false;
                    yield return new WaitForSeconds(2);

                    RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Rigidbody>().isKinematic = true;

                    Debug.Log("Current Lap of Third Car: " + RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Statistics>().lap);

                    RankManager.instance.racerRanks[i].racer.gameObject.transform.position = Vector3.Lerp(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker3.position, fracJourney3);
                    //yield return new WaitForSeconds(2);
                   
                }
            }
        }

        //RaceManager.instance.raceCompleted = true;
    }
}

