using UnityEngine;
using System.Collections;

public class manager : MonoBehaviour {
	private int lapNumber = 0; 
	private bool halfPointMet = false; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void increaseLapNumber(){
		if (halfPointMet) {
			lapNumber++;
			halfPointMet = false; 
		} else {
			return; 
		}

		if (lapNumber == 3) {
			// freeze everything 
			// Access the rigid body on the car and set isCinematic to true. 

			Application.LoadLevel("winners");
			Debug.Log("Third lap reached ! "); 
			lapNumber = 0; 
		}

		Debug.Log ("Lap Number : " + this.getLapNumber());
	}

	public int getLapNumber(){
		return lapNumber; 
	}

	public void halfLapPassed(){
		Debug.Log ("HalfLap point passed !");
		halfPointMet = true; 
	}
}
