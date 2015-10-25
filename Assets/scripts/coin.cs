using UnityEngine;
using System.Collections;

public class coin : MonoBehaviour {

	private float newXpos;
	private float newYpos;
	private float newZpos;
	private bool falling = true;
	public Transform explosion;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.forward * Time.deltaTime * 50);
		if (falling) {
			transform.Translate(Vector3.forward * Time.deltaTime * 2);
		}
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log("other = " + other);
		if (other.name == "hilly track"){
			falling = false;
		}

		if (other.tag == "barrier"){
			falling = false;
		}

		if (other.tag == "Player"){
			Instantiate(explosion, transform.position, Quaternion.identity);
			newXpos = Random.Range(10.0f,140.0f);
			newZpos = Random.Range(5.0f,70.0f);
			newYpos = 8.0f;
			transform.position = new Vector3 (newXpos, newYpos, newZpos);
			falling = true;
		}
	}
	
}
