﻿//PlayerControl.cs handles user input to control the car

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	
	private CarController car_controller;
	
	void Awake () {
		car_controller = GetComponent<CarController>();
	}
	
	void Update ()
    {
        DesktopControl();

    }
	
	void DesktopControl(){
		
        //Keyboard:
		car_controller.steerInput = Mathf.Clamp(Input.GetAxis("Horizontal"),-1,1);
		car_controller.motorInput = Mathf.Clamp01(Input.GetAxis("Vertical"));
		car_controller.brakeInput = Mathf.Clamp01(-Input.GetAxis("Vertical"));

        //XBox Controller:
        //car_controller.steerInput = Input.GetAxis("AnalogueSteerXBox360");
        //car_controller.motorInput = -Input.GetAxis("TriggersXBox360");
        //car_controller.brakeInput = Input.GetAxis("TriggersXBox360");
        //
        //Respawn the car if we press the Enter key
        if (Input.GetKey(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick2Button6) || Input.GetKeyDown(KeyCode.Joystick1Button8))
        {
				Respawn();
		}
	}
	
	public void Respawn(){
		if(RaceManager.instance.raceStarted)
			RaceManager.instance.RespawnRacer(transform,GetComponent<Statistics>().lastPassedNode);
	}
}
