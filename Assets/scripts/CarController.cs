using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarController : MonoBehaviour {
	
	public enum Propulsion{RWD,FWD}
	
	public Propulsion _propulsion = Propulsion.RWD;
	
	//Wheel Transforms
	public Transform FL_Wheel;
	public Transform FR_Wheel;
	public Transform RL_Wheel;
	public Transform RR_Wheel;
	private List<Transform> wheeltransformList = new List<Transform>();
	
	//Wheel Colliders
	public WheelCollider FL_WheelCollider;
	public WheelCollider FR_WheelCollider;
	public WheelCollider RL_WheelCollider;
	public WheelCollider RR_WheelCollider;
	private List<WheelCollider> wheelcolliderList = new List<WheelCollider>();
	
	//Engine values
	public float engineTorque = 800.0f; //avoid setting this value too high inorder to not overpower the wheels!
	public float maxSteerAngle = 30.0f;
	public float brakePower = 20000.0f;
	public float topSpeed = 150.0f;
	public float boost = 100.0f;
	public float currentSpeed;
	private float currentSteerAngle;
	
	//Gear values
	public int numberOfGears = 6;
	public int currentGear;
	private float[] gearRatio;
	
	//Stability
	private Rigidbody rigid;
	public Vector3 centerOfMass;
	public float antiRollAmount = 8000.0f;
	public float downforce = 50.0f;
	[Range(0,1)]public float steerHelper = 0.5f;
	[Range(0,1)]public float traction = 0.5f;
	
	//Sounds will be here
	
	//Bools
	public bool controllable;
	public bool reversing;
	public bool canSlipstream;
	
	//Misc
	public GameObject brakelightGroup;
	public GameObject steeringWheel;
	public float slipstreamRayHeight = 0.5f; //height of the slipstream ray
	private float speedLimit;
	//private float impactForce = 5.0f; //the impact force required to play a collision sound
	private Vector3 velocityDir;
	private float currentRotation;
	private float drag = 0.0f;
	
	//Input values
	public float motorInput;
	public float brakeInput;
	public float steerInput;
	
	void Start () {
		Initialize();
	}
	
	void Initialize(){
		
		rigid = GetComponent<Rigidbody>();
		rigid.centerOfMass = centerOfMass;
		//rigid.interpolation = RigidbodyInterpolation.None;
		
		//Add wheel transfoms/colliders to the lists
		wheeltransformList.Add(FL_Wheel); wheelcolliderList.Add(FL_WheelCollider);
		wheeltransformList.Add(FR_Wheel); wheelcolliderList.Add(FR_WheelCollider);
		wheeltransformList.Add(RL_Wheel); wheelcolliderList.Add(RL_WheelCollider);
		wheeltransformList.Add(RR_Wheel); wheelcolliderList.Add(RR_WheelCollider);
		
		//Calculate gearRatio
		gearRatio = new float[numberOfGears];
		for(int i = 0; i < numberOfGears; i++){
			gearRatio[i] = Mathf.Lerp(0, topSpeed, ((float)i/(float)(numberOfGears)));
		}
		gearRatio[numberOfGears-1] = topSpeed + 50.0f; //ensure the last gear doesn't exceed topSpeed!
		
		//set up audio here
		
		//set controllable to false
		if(controllable && RaceManager.instance)
			controllable = false;
		
	}
	
	
	
	void Update(){
		ShiftGears();
	}
	
	
	
	void FixedUpdate () {
		
		if(controllable){
			Drive();
            
                Brake();
		}
		else{
			Stop();
		}
		
		WheelAllignment();
		ApplyDownforce();
		StabilizerBars();
	}
	
	
	
	void Drive(){
		
		
		switch(_propulsion){
			
		//Rear wheel drive
		case Propulsion.RWD : 
			
			if(currentSpeed <= speedLimit){
				if(!reversing){
					RL_WheelCollider.motorTorque = engineTorque * motorInput;
					RR_WheelCollider.motorTorque = engineTorque * motorInput;
				}
				else{
					RL_WheelCollider.motorTorque = engineTorque * -brakeInput;
					RR_WheelCollider.motorTorque = engineTorque * -brakeInput;
				}
			}
			else{
				rigid.velocity = (speedLimit/2.237f) * rigid.velocity.normalized;
				RL_WheelCollider.motorTorque = 0;
				RR_WheelCollider.motorTorque = 0;
			}
			
			break;
			
		//Front wheel drive
		case Propulsion.FWD :
			
			if(currentSpeed <= speedLimit){
				if(!reversing){
					FL_WheelCollider.motorTorque = engineTorque * motorInput;
					FR_WheelCollider.motorTorque = engineTorque * motorInput;
				}
				else{
					FL_WheelCollider.motorTorque = engineTorque * -brakeInput;
					FR_WheelCollider.motorTorque = engineTorque * -brakeInput;
				}
			}
			else{
				rigid.velocity = (speedLimit/2.237f) * rigid.velocity.normalized;
				FL_WheelCollider.motorTorque = 0;
				FR_WheelCollider.motorTorque = 0;
			}
			
			break;
		}
		
		//Nitro Boost
		if( Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Joystick2Button0) || Input.GetKey(KeyCode.Joystick1Button1) && tag.Equals("Player"))
        {
			rigid.AddForce(transform.forward * boost, ForceMode.Acceleration);
		}
		
		//Steering
		//Reduce our steer angle depending on how fast the car is moving
		currentSteerAngle = Mathf.Lerp(maxSteerAngle,(maxSteerAngle/2),(currentSpeed/(topSpeed * 2.0f)));
		
		FL_WheelCollider.steerAngle = Mathf.Clamp((currentSteerAngle * steerInput), -maxSteerAngle, maxSteerAngle);
		FR_WheelCollider.steerAngle = Mathf.Clamp((currentSteerAngle * steerInput), -maxSteerAngle, maxSteerAngle);
		
		SteerHelper();
		Traction();
		
		//calculate speed & drag values
		currentSpeed = CalculateSpeed();
		rigid.drag = CalculateDrag();
		
		//Check for reverse
		velocityDir = transform.InverseTransformDirection(rigid.velocity);
		
		if(brakeInput > 0  && velocityDir.z <= 0.01f){
			reversing = true;
			speedLimit = 25.0f;
		}
		else{
			reversing = false;
			speedLimit = topSpeed;
		}
		
		//Handle steering wheel rotation
		if(steeringWheel)
			steeringWheel.transform.rotation = transform.rotation * Quaternion.Euler( 0, 0, (FR_WheelCollider.steerAngle) * -2.0f);
		
		//Handle slipstream
		if(canSlipstream)
			Slipstream();
	}
	
	void Brake(){
		//Footbrake
		if(!reversing && brakeInput > 0.0f)
        {
            
            //add a backward force to help stop the car
            rigid.AddForce(-transform.forward * 250);
			
			if(_propulsion == Propulsion.RWD){
				RL_WheelCollider.brakeTorque = brakePower * brakeInput;
				RR_WheelCollider.brakeTorque = brakePower * brakeInput;
				RL_WheelCollider.motorTorque = 0;
				RR_WheelCollider.motorTorque = 0;
			}
			else{
				FL_WheelCollider.brakeTorque = brakePower * brakeInput;
				FR_WheelCollider.brakeTorque = brakePower * brakeInput;
				FL_WheelCollider.motorTorque = 0;
				FR_WheelCollider.motorTorque = 0;
			}
		}
		
		else{
				RL_WheelCollider.brakeTorque = 0;
				RR_WheelCollider.brakeTorque = 0;
				FL_WheelCollider.brakeTorque = 0;
				FR_WheelCollider.brakeTorque = 0;
		}
		
		//Decelerate
		if(motorInput == 0 && brakeInput == 0 && rigid.velocity.magnitude > 1.0f){
			if(velocityDir.z >= 0.01f)
				rigid.AddForce(-transform.forward * 250);
			else
				rigid.AddForce(transform.forward * 250);
		}
		
		//Activate brake lights if braking
		if(brakelightGroup)
			brakelightGroup.SetActive(brakeInput > 0);
	}   
        
    private float CalculateSpeed(){
		//Calculate currentSpeed(MPH)
		currentSpeed = GetComponent<Rigidbody>().velocity.magnitude * 2.237f;
		//Round currentSpeed
		currentSpeed = Mathf.Round(currentSpeed);
		//Never return a negative value
		return Mathf.Abs(currentSpeed);
	}
	
	private float CalculateDrag(){
		//increase drag as the car gains speed
		drag = (currentSpeed/topSpeed) / 10;
		return Mathf.Clamp(drag, 0.0f, 0.1f);
	}
	
	
	void ShiftGears(){
		int i;
		
		for (i = 0; i < gearRatio.Length; i ++ ) {
			if ( currentSpeed < gearRatio[i]) {
				break;
			}
			currentGear = i + 1;
		}
		
		float minimumGearValue = 0.0f;
		float maximumGearValue = 0.0f;
		
		if (i == 0){
			minimumGearValue = 0;
		}
		else {
			minimumGearValue = gearRatio[i-1];
		}
		maximumGearValue = gearRatio[i];
	}
	
	
	void WheelAllignment(){
		
		for(int i = 0; i < wheelcolliderList.Count; i++){
			
			Quaternion rot;
			Vector3 pos;
			
			wheelcolliderList[i].GetWorldPose(out pos, out rot);
			
			//Set rotation & position of the wheels
			wheeltransformList[i].rotation = rot;
			wheeltransformList[i].position = pos; 
			
		}
	}
	
	
	public void StabilizerBars (){
		
		WheelHit FrontWheelHit;
		
		float travelFL = 1.0f;
		float travelFR = 1.0f;
		
		bool groundedFL= FL_WheelCollider.GetGroundHit(out FrontWheelHit);
		
		if (groundedFL)
			travelFL = (-FL_WheelCollider.transform.InverseTransformPoint(FrontWheelHit.point).y - FL_WheelCollider.radius) / FL_WheelCollider.suspensionDistance;
		
		bool groundedFR= FR_WheelCollider.GetGroundHit(out FrontWheelHit);
		
		if (groundedFR)
			travelFR = (-FR_WheelCollider.transform.InverseTransformPoint(FrontWheelHit.point).y - FR_WheelCollider.radius) / FR_WheelCollider.suspensionDistance;
		
		float antiRollForceFront= (travelFL - travelFR) * antiRollAmount;
		
		if (groundedFL)
			rigid.AddForceAtPosition(FL_WheelCollider.transform.up * -antiRollForceFront, FL_WheelCollider.transform.position); 
		if (groundedFR)
			rigid.AddForceAtPosition(FR_WheelCollider.transform.up * antiRollForceFront, FR_WheelCollider.transform.position); 
		
		WheelHit RearWheelHit;
		
		float travelRL = 1.0f;
		float travelRR = 1.0f;
		
		bool groundedRL= RL_WheelCollider.GetGroundHit(out RearWheelHit);
		
		if (groundedRL)
			travelRL = (-RL_WheelCollider.transform.InverseTransformPoint(RearWheelHit.point).y - RL_WheelCollider.radius) / RL_WheelCollider.suspensionDistance;
		
		bool groundedRR= RR_WheelCollider.GetGroundHit(out RearWheelHit);
		
		if (groundedRR)
			travelRR = (-RR_WheelCollider.transform.InverseTransformPoint(RearWheelHit.point).y - RR_WheelCollider.radius) / RR_WheelCollider.suspensionDistance;
		
		float antiRollForceRear= (travelRL - travelRR) * antiRollAmount;
		
		if (groundedRL)
			rigid.AddForceAtPosition(RL_WheelCollider.transform.up * -antiRollForceRear, RL_WheelCollider.transform.position); 
		if (groundedRR)
			rigid.AddForceAtPosition(RR_WheelCollider.transform.up * antiRollForceRear, RR_WheelCollider.transform.position);
		
		if (groundedRR && groundedRL && currentSpeed > 5.0f)
			rigid.AddRelativeTorque((Vector3.up * (steerInput)) * 5000f);
		
	}
	
	
	void ApplyDownforce(){
		rigid.AddForce(-transform.up*downforce*rigid.velocity.magnitude);
	}
	
	
	void Traction(){
	switch(_propulsion){
		case Propulsion.RWD :
		WheelFrictionCurve rearWheelStiffness =  RL_WheelCollider.sidewaysFriction;
		rearWheelStiffness.stiffness = traction + 1.0f;
		
		RL_WheelCollider.sidewaysFriction = rearWheelStiffness;
		RR_WheelCollider.sidewaysFriction = rearWheelStiffness;
		break;
		
		case Propulsion.FWD :
		WheelFrictionCurve frontWheelStiffness =  FL_WheelCollider.sidewaysFriction;
		frontWheelStiffness.stiffness = traction + 1.0f;
		
		FL_WheelCollider.sidewaysFriction = frontWheelStiffness;
		FR_WheelCollider.sidewaysFriction = frontWheelStiffness;
		break;
		
		}
	}
	
	void SteerHelper(){
		
		for (int i = 0; i < 4; i++){
			WheelHit wheelhit;
			wheelcolliderList[i].GetGroundHit(out wheelhit);
			if (wheelhit.normal == Vector3.zero)
				return;
		}
		
		if (Mathf.Abs(currentRotation - transform.eulerAngles.y) < 10f){
			var turnadjust = (transform.eulerAngles.y - currentRotation) * (steerHelper / 2);
			Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
				rigid.velocity = velRotation * rigid.velocity;
		}
		
		currentRotation = transform.eulerAngles.y;
	}
	
	
	void Slipstream(){
		
		Vector3 fwd = transform.TransformDirection ( new Vector3(0, 0, 1));
		RaycastHit hit;
		
		//Draw the rays in edit mode
		#if UNITY_EDITOR
		Debug.DrawRay (new Vector3(transform.position.x - 1,transform.position.y + slipstreamRayHeight,transform.position.z),fwd * 20,Color.cyan);
		Debug.DrawRay (new Vector3(transform.position.x,transform.position.y + slipstreamRayHeight,transform.position.z),fwd * 20,Color.cyan);
		Debug.DrawRay (new Vector3(transform.position.x + 1,transform.position.y + slipstreamRayHeight,transform.position.z),fwd * 20,Color.cyan);
		#endif
		
		//Cast 3 rays from the car that check whether there is a car infront and give the car a slipstream boost of 100.0f
		
		//Left ray
		if(Physics.Raycast (new Vector3(transform.position.x - 1.0f,transform.position.y + slipstreamRayHeight,transform.position.z), Quaternion.AngleAxis(0, transform.up) * fwd, out hit, 20)) {
			if(hit.collider.transform.root.GetComponent<CarController>() && currentSpeed > 25 && motorInput > 0){
				rigid.AddForce(Vector3.forward * 500.0f);
			}
		}
		
		//Center ray
		if(Physics.Raycast (new Vector3(transform.position.x,transform.position.y + slipstreamRayHeight,transform.position.z), Quaternion.AngleAxis(0, transform.up) * fwd, out hit, 20)) {
			if(hit.collider.transform.root.GetComponent<CarController>() && currentSpeed > 25 && motorInput > 0){
				rigid.AddForce(Vector3.forward * 500.0f);
			}
		}
		
		//Right ray
		if(Physics.Raycast (new Vector3(transform.position.x + 1.0f,transform.position.y + slipstreamRayHeight,transform.position.z), Quaternion.AngleAxis(0, transform.up) * fwd, out hit, 20)) {
			if(hit.collider.transform.root.GetComponent<CarController>() && currentSpeed > 25 && motorInput > 0){
				rigid.AddForce(Vector3.forward * 500.0f);
			}
		}
	}
	
	
	void Stop(){
		
		motorInput = 0.0f;
		steerInput = 0.0f;
		currentSpeed = CalculateSpeed();
		
		FL_WheelCollider.motorTorque = 0;
		FR_WheelCollider.motorTorque = 0;
		RL_WheelCollider.motorTorque = 0;
		RR_WheelCollider.motorTorque = 0;
		
		RL_WheelCollider.brakeTorque = brakePower;
		RR_WheelCollider.brakeTorque = brakePower;
		FL_WheelCollider.brakeTorque = brakePower;
		FR_WheelCollider.brakeTorque = brakePower;
	}
}
