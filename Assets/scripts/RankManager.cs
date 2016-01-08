//RankManager.cs handles setting each racer's position/rank
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankManager : MonoBehaviour {
	
	[System.Serializable]
	public class Ranker : IComparer<Ranker>{
		public GameObject racer;
		public float raceCompletion;
		
		public int Compare(Ranker x, Ranker y){
			
				return x.raceCompletion.CompareTo(y.raceCompletion);
		}
	}
	
	public static RankManager instance;
	[HideInInspector]public List<Ranker> racerRanks = new List<Ranker>(new Ranker[100]);//allow upto 100 racers
	[HideInInspector]public List<ProgressTracker> racerStats = new List<ProgressTracker>();
	[HideInInspector]public int totalRacers; //number of racers when the race begins
	[HideInInspector]public int currentRacers; //number of racers that are currently not knocked out
	
	void Awake () {
		//create an instance
		instance = this;
	}
	
	
	void Start () {
		InvokeRepeating("SetCarRank",1.0f,0.5f);
	}
	
	
	//Finds the number of racers in the race.
	public void RefreshRacerCount(){
		Statistics[] m_racers = GameObject.FindObjectsOfType(typeof(Statistics)) as Statistics[];
		
		totalRacers = m_racers.Length;
        Debug.Log("Total racers: " + totalRacers);
		
		for(int i = 0; i < m_racers.Length; i++){
			if(m_racers[i].knockedOut == false){
				if(!racerStats.Contains(m_racers[i].GetComponent<ProgressTracker>())){
					racerStats.Add(m_racers[i].GetComponent<ProgressTracker>());
				}
			}
			else{
				racerStats.Remove(m_racers[i].GetComponent<ProgressTracker>());
			}
		}
		
		//Resize the list
		racerRanks.RemoveRange(totalRacers, racerRanks.Count - totalRacers);
		
		currentRacers = racerStats.Count;
	}
	
	
	void Update () {
		//Fill & sort the list in order
		for(int i = 0; i < currentRacers; i++){
			
			racerRanks[i].racer = racerStats[i].gameObject;
			racerRanks[i].raceCompletion = racerStats[i].raceCompletion - ((float)racerStats[i].GetComponent<Statistics>().rank / 1000);
			
		}
		
		Ranker m_ranker = new Ranker();
		racerRanks.Sort(m_ranker);
		racerRanks.Reverse();
	}
	
	
	//Sets the car ranks accoding to the sorted list
	public void SetCarRank(){
		for(int r = 0; r < currentRacers; r++){
			if(racerRanks[r].racer){
				racerRanks[r].racer.GetComponent<Statistics>().rank = r + 1;
                
			}
            
		}
	}

    public void Reset()
    {
        totalRacers = 0;
    }
}
