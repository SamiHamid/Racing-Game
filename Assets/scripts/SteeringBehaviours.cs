//OpponentControl.cs handles AI behaviour mainly by controlling the motorInput & steerInput values in Car_Controller.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class SteeringBehaviours : MonoBehaviour {
	
	private CarController car_controller;
	private Transform target;
	private bool shouldBrake;
	
	[Header("Behaviour")]
	//Behaviour configurations
	[Range(0,1f)]public float accelerationSensitivity = 1.0f;
	[Range(0,1)]public float brakeSensitivity = 1.0f;  
	[Range(0,0.1f)]public float steerSensitivity = 0.01f; 
	[Range(0,1)]public float avoidanceSensitivity = 0.1f;
	[Range(0,25)]public float maxWanderDistance = 10.0f;
	[Range(0,0.2f)]public float wanderRate = 0.1f;
	[Range(0,1)]public float cautionSpeed = 0.3f;
	public float cautionAmount = 30.0f;
	private float cautionAngle = 45.0f; //angle to treat as cautious. Default set to 45°.
	public bool randomMaxWanderDistance;//sets a random "MaxWanderDistance" on start
	public bool randomWanderRate;//sets a random "WanderRate" on start
	
	[Space(5)]
	
	[Header("Sensors")]
	//Raycasts
	public float forward_rayDistance = 8.0f;
	public float wideAngle_rayDistance = 8.0f;
	public float shortAngle_rayDistance = 10.0f;
	public float sideways_rayDistance = 4.0f;
	public float rayHeight = 0.5f;
	private float stuckTimer;

	//Behaviour values
	private float newSteer;
	private float avoidanceSteer;
	private float randomValue;
	private float requiredSpeed;
	private float aproachingAngle;
	private float spinningAngle;
	private float cautionRequired;
	private float throttleSensitvity;
	private float throttle;
	private float targetAngle;
	private Vector3 offsetTargetPos;
	private Vector3 steerVector;
	
	
	void Awake(){
		car_controller = GetComponent<CarController>();
		randomValue = Random.value * 100;
	}
	
	void Start(){
		if(randomMaxWanderDistance)
			maxWanderDistance = Random.Range(0,20);
		
		if(randomWanderRate)
			wanderRate = Random.Range(0.01f,0.2f);
	}
	
	void Update () {
		Sensors();
		CheckIfStuck();
	}
	
	
	void FixedUpdate(){
		if(!RaceManager.instance || !GetComponent<Statistics>())
			return;
		
		Navigate();
	}
	
	
	void Navigate(){
		
		//Set the target
		target = GetComponent<Statistics>().target;

		//Handle AI Beahviour
		Vector3 fwd = transform.forward;
		
		requiredSpeed = car_controller.topSpeed;
		
		aproachingAngle = Vector3.Angle(target.forward, fwd);
		
		spinningAngle = GetComponent<Rigidbody>().angularVelocity.magnitude * cautionAmount;
		
		cautionRequired = Mathf.InverseLerp(0, cautionAngle,Mathf.Max(spinningAngle,aproachingAngle));  
		
		requiredSpeed = Mathf.Lerp(car_controller.topSpeed, car_controller.topSpeed*(cautionSpeed), cautionRequired);  
		
		offsetTargetPos = target.position;
		
		offsetTargetPos += target.right * (Mathf.PerlinNoise(Time.time * wanderRate, randomValue) * 2 - 1) * maxWanderDistance;

		throttleSensitvity = (requiredSpeed < car_controller.currentSpeed) ? brakeSensitivity : accelerationSensitivity;
		
		throttle = Mathf.Clamp((requiredSpeed - car_controller.currentSpeed) * throttleSensitvity, -1, 1);
		
		throttle *= 1 + (Mathf.PerlinNoise(Time.time * 1.0f, randomValue));
		
		steerVector = transform.InverseTransformPoint(offsetTargetPos);
		
		targetAngle = Mathf.Atan2(steerVector.x, steerVector.z)*Mathf.Rad2Deg;
		
		newSteer = Mathf.Clamp(targetAngle * steerSensitivity, -1, 1)*Mathf.Sign(car_controller.currentSpeed);

		//Finaly, feed the input values
		FeedInput(throttle, throttle, newSteer);
	}
	
	
	void FeedInput(float motor, float brake, float steer){
		
		//Assign motor values
		if(!shouldBrake){
			car_controller.motorInput = motor = Mathf.Clamp(motor, 0, 1);
			car_controller.brakeInput = brake = -1 * Mathf.Clamp(brake, -1, 0);
		}
		else{
			car_controller.motorInput = 0.0f;
			car_controller.brakeInput = 1.0f; 
		}
		
		
		//Assign steer value
		if(!car_controller.reversing){
			car_controller.steerInput = Mathf.Clamp(steer + avoidanceSteer,-1,1);
		}
		else{
			car_controller.steerInput = Mathf.Clamp(-(steer + avoidanceSteer),-1,1);
		}
	}
	
	
	void Sensors () {
		Vector3 fwd = transform.TransformDirection ( new Vector3(0, 0, 1));
		RaycastHit hit;
		
		avoidanceSteer = 0.0f;
		
		//Draw the rays
		#if UNITY_EDITOR
		
		Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(0, transform.up) * fwd * forward_rayDistance, Color.white);
		
		Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(15, transform.up) * fwd * shortAngle_rayDistance, Color.white);
		Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(-15, transform.up) * fwd * shortAngle_rayDistance, Color.white);
		
		Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(7, transform.up) * fwd * shortAngle_rayDistance, Color.white);
		Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(-7, transform.up) * fwd * shortAngle_rayDistance, Color.white);
		
		Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(30, transform.up) * fwd * wideAngle_rayDistance, Color.white);
		Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(-30, transform.up) * fwd * wideAngle_rayDistance, Color.white);
		
		Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(90, transform.up) * fwd * sideways_rayDistance, Color.white);
		Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(-90, transform.up) * fwd * sideways_rayDistance, Color.white);
		
		#endif
		
		//straight forward ray
		if(Physics.Raycast (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(0, transform.up) * fwd, out hit, forward_rayDistance)) {
			
			if(hit.collider.gameObject.layer != LayerMask.NameToLayer("IgnoreCollision")){
				Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(0, transform.up) * fwd * forward_rayDistance, Color.red);
				
				//Brake when there is something infront of us
				shouldBrake = true;
			}
		}
		else{
			shouldBrake = false;
		}
		
		//right_angled wide ray
		if (Physics.Raycast (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(30, transform.up) * fwd, out hit, wideAngle_rayDistance)) {
			if(hit.collider.gameObject.layer != LayerMask.NameToLayer("IgnoreCollision")){
				
				Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(30, transform.up) * fwd * wideAngle_rayDistance, Color.red);
				
				avoidanceSteer = Mathf.Lerp(avoidanceSteer,-(avoidanceSensitivity),(hit.distance / wideAngle_rayDistance));
			}
		}
		
		//left_angled wide ray
		if (Physics.Raycast(new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(-30, transform.up) * fwd, out hit, wideAngle_rayDistance)) {
			if(hit.collider.gameObject.layer != LayerMask.NameToLayer("IgnoreCollision")){
				
				Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(-30, transform.up) * fwd * wideAngle_rayDistance, Color.red);
				
				avoidanceSteer = Mathf.Lerp(avoidanceSteer,avoidanceSensitivity,(hit.distance / wideAngle_rayDistance));
			}
		}
		
		//right_angled forward ray
		if (Physics.Raycast(new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(15, transform.up) * fwd, out hit, shortAngle_rayDistance)) {
			if(hit.collider.gameObject.layer != LayerMask.NameToLayer("IgnoreCollision")){
				
				Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(15, transform.up) * fwd * shortAngle_rayDistance, Color.red);
				
				avoidanceSteer = Mathf.Lerp(avoidanceSteer,-(avoidanceSensitivity/2),(hit.distance / forward_rayDistance));
			}	
		}
		
		//left_angled forward ray
		if (Physics.Raycast(new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(-15, transform.up) * fwd, out hit, shortAngle_rayDistance)) {
			if(hit.collider.gameObject.layer != LayerMask.NameToLayer("IgnoreCollision")){
				
				Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(-15, transform.up) * fwd * shortAngle_rayDistance, Color.red);
				
				avoidanceSteer = Mathf.Lerp(avoidanceSteer,(avoidanceSensitivity/2),(hit.distance / forward_rayDistance));
			}
		}
		
		//right_angled short forward ray
		if (Physics.Raycast(new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(7, transform.up) * fwd, out hit, shortAngle_rayDistance)) {
			if(hit.collider.gameObject.layer != LayerMask.NameToLayer("IgnoreCollision")){
				
				Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(7, transform.up) * fwd * shortAngle_rayDistance, Color.red);
				
				avoidanceSteer = Mathf.Lerp(avoidanceSteer,-(avoidanceSensitivity/2),(hit.distance / forward_rayDistance));
			}	
		}
		
		//left_angled short forward ray
		if (Physics.Raycast(new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(-7, transform.up) * fwd, out hit, shortAngle_rayDistance)) {
			if(hit.collider.gameObject.layer != LayerMask.NameToLayer("IgnoreCollision")){
				
				Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(-7, transform.up) * fwd * shortAngle_rayDistance, Color.red);
				
				avoidanceSteer = Mathf.Lerp(avoidanceSteer,(avoidanceSensitivity/2),(hit.distance / forward_rayDistance));
			}
		}
		
		//right sideway ray
		if (Physics.Raycast (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(90, transform.up) * fwd, out hit, sideways_rayDistance)) {
			if(hit.collider.gameObject.layer != LayerMask.NameToLayer("IgnoreCollision")){
				
				Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(90, transform.up) * fwd * sideways_rayDistance, Color.red);
				
				avoidanceSteer = Mathf.Lerp(avoidanceSteer,-(avoidanceSensitivity),(hit.distance / sideways_rayDistance));
			}
		}
		
		//left sideway ray
		if (Physics.Raycast (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(-90, transform.up) * fwd, out hit, sideways_rayDistance)) {
			if(hit.collider.gameObject.layer != LayerMask.NameToLayer("IgnoreCollision")){
				
				Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + rayHeight,transform.position.z), Quaternion.AngleAxis(-90, transform.up) * fwd * sideways_rayDistance, Color.red);
				
				avoidanceSteer = Mathf.Lerp(avoidanceSteer,avoidanceSensitivity,(hit.distance / sideways_rayDistance));
			}	
		}
	}
	
	void CheckIfStuck(){
		if(!RaceManager.instance)
			return;
		
		//Add to the respawn timer incase the AI gets stuck
		if(RaceManager.instance.raceStarted && car_controller.currentSpeed <= 5.0f){
			stuckTimer += Time.deltaTime;
			if(stuckTimer >= 10.0f){
				RaceManager.instance.RespawnRacer(transform,GetComponent<Statistics>().lastPassedNode,3.0f);
				stuckTimer = 0.0f;
			}
		}
		else{
			stuckTimer = 0.0f;
		}
	}
	
	#if UNITY_EDITOR
	void OnDrawGizmos(){
		//Draw a gizmo at our offset taregt(helps you find good wander settings for your race track)
		if(Application.isPlaying)
			Gizmos.DrawSphere(offsetTargetPos,1);
	}
	#endif
}
