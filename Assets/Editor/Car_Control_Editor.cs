using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CarController))]
public class Car_Control_Editor : Editor {

	
	CarController m_target;
	
	public void OnEnable () {
    m_target = (CarController)target;
	}
	
	public override void OnInspectorGUI(){
	GUILayout.BeginVertical("Box");
	GUILayout.Box("Wheel Settings",EditorStyles.boldLabel);
	EditorGUILayout.Space();
	
	m_target._propulsion = (CarController.Propulsion)EditorGUILayout.EnumPopup("Propulsion",m_target._propulsion);
	
	EditorGUILayout.Space();
	
	m_target.FL_Wheel = EditorGUILayout.ObjectField("Front Left Wheel Transform",m_target.FL_Wheel,typeof(Transform),true) as Transform;
	m_target.FR_Wheel = EditorGUILayout.ObjectField("Front Right Wheel Transform",m_target.FR_Wheel,typeof(Transform),true) as Transform;
	m_target.RL_Wheel = EditorGUILayout.ObjectField("Rear Left Wheel Transform",m_target.RL_Wheel,typeof(Transform),true) as Transform;
	m_target.RR_Wheel = EditorGUILayout.ObjectField("Rear Right Wheel Transform",m_target.RR_Wheel,typeof(Transform),true) as Transform;
	
	EditorGUILayout.Space();
	
	m_target.FL_WheelCollider = EditorGUILayout.ObjectField("Front Left WheelCollider",m_target.FL_WheelCollider,typeof(WheelCollider),true) as WheelCollider;
	m_target.FR_WheelCollider = EditorGUILayout.ObjectField("Front Right WheelCollider",m_target.FR_WheelCollider,typeof(WheelCollider),true) as WheelCollider;
	m_target.RL_WheelCollider = EditorGUILayout.ObjectField("Rear Left WheelCollider",m_target.RL_WheelCollider,typeof(WheelCollider),true) as WheelCollider;
	m_target.RR_WheelCollider = EditorGUILayout.ObjectField("Rear Right WheelCollider",m_target.RR_WheelCollider,typeof(WheelCollider),true) as WheelCollider;
	
	GUILayout.EndVertical();
	
	EditorGUILayout.Space();
	
	GUILayout.BeginVertical("Box");
	GUILayout.Box("Engine Settings",EditorStyles.boldLabel);
	EditorGUILayout.Space();
	
	m_target.engineTorque = EditorGUILayout.FloatField("Engine Torque",m_target.engineTorque);
	m_target.maxSteerAngle = EditorGUILayout.FloatField("Max Steer Angle",m_target.maxSteerAngle);
	m_target.topSpeed = EditorGUILayout.FloatField("Top Speed",m_target.topSpeed);
	m_target.brakePower = EditorGUILayout.FloatField("Brake Power",m_target.brakePower);
	m_target.numberOfGears = EditorGUILayout.IntField("Total Gears",m_target.numberOfGears);
	m_target.boost = EditorGUILayout.FloatField("Boost",m_target.boost);
    m_target.controllable = EditorGUILayout.Toggle("Controllable",m_target.controllable);
    m_target.canSlipstream = EditorGUILayout.Toggle("Slipstream",m_target.canSlipstream);
    GUILayout.EndVertical();
    
    EditorGUILayout.Space();
    
    GUILayout.BeginVertical("Box");
	GUILayout.Box("Stability Settings",EditorStyles.boldLabel);
	EditorGUILayout.Space();
    //m_target.centerOfMass = EditorGUILayout.ObjectField("Center Of Mass",m_target.centerOfMass,typeof(Transform),true) as Transform;
    m_target.centerOfMass = EditorGUILayout.Vector3Field("Center Of Mass",m_target.centerOfMass);
    EditorGUILayout.Space();
    m_target.antiRollAmount = EditorGUILayout.FloatField("Anti Roll Amount",m_target.antiRollAmount);
    m_target.downforce = EditorGUILayout.FloatField("Downforce",m_target.downforce);
	m_target.steerHelper = EditorGUILayout.Slider("Steer Helper",m_target.steerHelper,0.0f,1.0f);
	m_target.traction = EditorGUILayout.Slider("Traction",m_target.traction,0.0f,1.0f);
    GUILayout.EndVertical();
    
	
    EditorGUILayout.Space();
    
    GUILayout.BeginVertical("Box");
	GUILayout.Box("Input",EditorStyles.boldLabel);
	EditorGUILayout.Space();
    EditorGUI.BeginDisabledGroup (true);
    m_target.motorInput = EditorGUILayout.FloatField("Motor Input",m_target.motorInput);
    m_target.brakeInput = EditorGUILayout.FloatField("Brake Input",m_target.brakeInput);
    m_target.steerInput = EditorGUILayout.FloatField("Steer Input",m_target.steerInput);
    EditorGUI.EndDisabledGroup();
    GUILayout.EndVertical();
    
    //Set dirty
    if(GUI.changed){ EditorUtility.SetDirty(m_target);}
	}
}
