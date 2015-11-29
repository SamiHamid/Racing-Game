//racer_name.cs is used to display opponent racer's names above them. 

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
public class RacerName : MonoBehaviour {
	
	public enum DisplayFormat{Rank,RankAndName,NameAndDistance,RankNameDistance}
	public DisplayFormat displayFormat;
	[HideInInspector]public Transform target; //this is automatically assigned when the race starts
	public Vector3 positionOffset = new Vector3(0,1.75f,0.5f);
	public float distanceToDisplay = 30.0f; //how far(in Meters) you have to be from a racer to see their names.
	public bool onlyShowRankAhead; //only appear if the opponent is 1 rank ahead
	private bool isPlayerAhead;
	private GameObject player;
	
	void Start(){
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	void Update () {
		
		Vector3 rot;
		
		float distanceFromPlayer = Vector3.Distance(transform.position,player.transform.position);
		
		//update its position & rotation
		transform.position = target.localPosition - positionOffset;
		rot = target.transform.eulerAngles;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, rot.y, transform.eulerAngles.z);
		
		//check if the player is ahead of the target(text is only displayed if the player is behind)
		Vector3 targetPos = player.transform.InverseTransformPoint(target.position);
		if(targetPos.z < 0){
			isPlayerAhead = true;
		}
		else{
			isPlayerAhead = false;
		}
		
		//update its text
		if(!onlyShowRankAhead){
			
			if(distanceFromPlayer <= distanceToDisplay && !isPlayerAhead){
				
				switch(displayFormat){
					
				case DisplayFormat.Rank : 
					this.GetComponent<TextMesh>().text = target.gameObject.GetComponent<Statistics>().rank.ToString();
					break;
					
				case DisplayFormat.RankAndName :
					this.GetComponent<TextMesh>().text = target.gameObject.GetComponent<Statistics>().rank + "  " + target.gameObject.name;
					break;
					
				case DisplayFormat.NameAndDistance :
					this.GetComponent<TextMesh>().text = target.gameObject.name + "  " + (int)distanceFromPlayer + "M";
					break;
					
				case DisplayFormat.RankNameDistance :
					this.GetComponent<TextMesh>().text = target.gameObject.GetComponent<Statistics>().rank + "  " + target.gameObject.name + "  " + (int)distanceFromPlayer + "M";
					break;
				}
			}
			else{
				this.GetComponent<TextMesh>().text = "";
			}
			
			//update renderer visibilty
			foreach(Transform t in transform){
				if(t.GetComponent<Renderer>()){
					t.GetComponent<Renderer>().enabled = distanceFromPlayer <= distanceToDisplay && !isPlayerAhead;
				}
			}
		}
		
		else{
			if(distanceFromPlayer <= distanceToDisplay && !isPlayerAhead && target.GetComponent<Statistics>().rank == player.GetComponent<Statistics>().rank - 1){
				
				switch(displayFormat){
					
				case DisplayFormat.Rank : 
					this.GetComponent<TextMesh>().text = target.gameObject.GetComponent<Statistics>().rank.ToString();
					break;
					
				case DisplayFormat.RankAndName :
					this.GetComponent<TextMesh>().text = target.gameObject.GetComponent<Statistics>().rank + "  " + target.gameObject.name;
					break;
					
				case DisplayFormat.NameAndDistance :
					this.GetComponent<TextMesh>().text = target.gameObject.name + "  " + (int)distanceFromPlayer + "M";
					break;
					
				case DisplayFormat.RankNameDistance :
					this.GetComponent<TextMesh>().text = target.gameObject.GetComponent<Statistics>().rank + "  " + target.gameObject.name + "  " + (int)distanceFromPlayer + "M";
					break;
				}
			}
			else{
				this.GetComponent<TextMesh>().text = "";
			}
			
			//update renderer visibilty
			foreach(Transform t in transform){
				if(t.GetComponent<Renderer>()){
					t.GetComponent<Renderer>().enabled = distanceFromPlayer <= distanceToDisplay && !isPlayerAhead && target.GetComponent<Statistics>().rank == player.GetComponent<Statistics>().rank - 1;
				}
			}
		}
	}
}
