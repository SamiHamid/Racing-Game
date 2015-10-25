using UnityEngine;
using System.Collections;

public class waypoints : MonoBehaviour {
	manager mManager; 
	// Use this for initialization
	void Start () {

		mManager = GameObject.FindGameObjectWithTag("manager").GetComponent<manager> (); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			if(this.tag == "startLine"){
				Debug.Log("Lap Number increased ! ");
				mManager.increaseLapNumber(); 
			}else if (this.tag == "halfLine"){
				mManager.halfLapPassed(); 
			}
		}
	}
}
