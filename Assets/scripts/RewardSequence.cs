﻿using UnityEngine;
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
        if (RaceManager.instance.raceCompleted)
            StartCoroutine(UpdateRaceStandings()); 
	}







    IEnumerator UpdateRaceStandings()
    {
        float distCovered = (Time.time - startTime) * speed;

        for (int i = 0; i < RankManager.instance.totalRacers; i++)
        {

            if (RankManager.instance.racerRanks[i].racer.GetComponent<Statistics>().rank == 1)
            {
                journeyLength1 = Vector3.Distance(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker1.position);
                float fracJourney1 = distCovered / journeyLength1;

                if (RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>().controllable == false)
                {
                    RankManager.instance.racerRanks[i].racer.gameObject.transform.position = Vector3.Lerp(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker1.position, fracJourney1);
                    yield return new WaitForSeconds(1);
                    //Destroy(RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Rigidbody>());
                    //Destroy(RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>());
                    //Destroy(RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<SteeringBehaviours>());
                }
            }

            else if (RankManager.instance.racerRanks[i].racer.GetComponent<Statistics>().rank == 2)
            {
                journeyLength2 = Vector3.Distance(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker2.position);
                float fracJourney2 = distCovered / journeyLength2;


                if (RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>().controllable == false)
                { 
                    RankManager.instance.racerRanks[i].racer.gameObject.transform.position = Vector3.Lerp(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker2.position, fracJourney2);
                    yield return new WaitForSeconds(1);
                    //Destroy(RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Rigidbody>());
                    //Destroy(RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>());
                    //Destroy(RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<SteeringBehaviours>());
                }
                
            }
            else if (RankManager.instance.racerRanks[i].racer.GetComponent<Statistics>().rank == 3)
            {
                journeyLength3 = Vector3.Distance(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker3.position);
                float fracJourney3 = distCovered / journeyLength3;


                if (RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>().controllable == false)
                {
                    RankManager.instance.racerRanks[i].racer.gameObject.transform.position = Vector3.Lerp(RankManager.instance.racerRanks[i].racer.gameObject.transform.position, endMarker3.position, fracJourney3);
                    yield return new WaitForSeconds(1);
                    //Destroy(RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Rigidbody>());
                    //Destroy(RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>());
                    //Destroy(RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<SteeringBehaviours>());
                }
            }
            
            Destroy(RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<Rigidbody>());
            Destroy(RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<CarController>());
            Destroy(RankManager.instance.racerRanks[i].racer.gameObject.GetComponent<SteeringBehaviours>());


        }
    }

    void DestroyComponents()
    {
        for (int i = 0; i < RankManager.instance.totalRacers; i++)
        {
            
        }
    }
}
