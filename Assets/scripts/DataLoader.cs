//DataLoader.cs simply loads race preferences and assigns them to the RaceManager.
using UnityEngine;
using System.Collections;

public class DataLoader : MonoBehaviour {

	public string ResourceFolder = "PlayerCars/"; //the name of the folder within the Resources folder where your cars are stored.
	public bool loadPreferences; //should player preferences set in the menu be loaded?
	
	private void OnEnable(){
		if(loadPreferences)
			LoadRacePreferences();
	}
	
	private void LoadRacePreferences(){
		//Load race prefernce if there is an active RaceManager
		if(!RaceManager.instance)
			return;
		
		//load player cars from the resources folder
		if(PlayerPrefs.HasKey("PlayerCar")){
			RaceManager.instance.playerCar = (GameObject)Resources.Load(ResourceFolder + PlayerPrefs.GetString("PlayerCar"));
		}
		
		//load laps
		if(PlayerPrefs.HasKey("Laps")){ 
			RaceManager.instance.totalLaps = PlayerPrefs.GetInt("Laps");
		}
		
    	//load racers
		RaceManager.instance.totalRacers = PlayerPrefs.GetInt("Opponents") + 1;
	}
}
