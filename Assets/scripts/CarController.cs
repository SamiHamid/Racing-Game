using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public WheelCollider[] m_WheelColliders = new WheelCollider[4];
    public GameObject[] m_WheelMeshes = new GameObject[4];
    public Vector3 m_CentreOfMass;
    public float m_MaximumSteerAngle;
    [Range(0, 1)] public float m_SteerHelper; 
    [Range(0, 1)] public float m_TractionControl;
    public bool Skidding { get; private set; }
    public float BrakeInput { get; private set; }
    public float CurrentSteerAngle { get { return m_SteerAngle; } }
    public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }
    public float MaxSpeed { get { return m_Topspeed; } }
    public float Revs { get; private set; }
    public float AccelInput { get; private set; }

    public float m_FullTorqueOverAllWheels;
    public float m_ReverseTorque;
    public float m_MaxHandbrakeTorque;
    public float m_Downforce = 100f;
    public float m_Topspeed = 200; 
    public static int NoOfGears = 5;
    public float m_RevRangeBoundary = 1f;
    public float m_SlipLimit;
    public float m_BrakeTorque;
    private Quaternion[] m_WheelMeshLocalRotations;
    private Vector3 m_Prevpos, m_Pos;
    private float m_SteerAngle;
    private int m_GearNum;
    private float m_GearFactor;
    private float m_OldRotation;
    private float m_CurrentTorque;
    private Rigidbody m_Rigidbody;
    private const float k_ReversingThreshold = 0.01f;

           
    private void Start()
    {
        m_WheelMeshLocalRotations = new Quaternion[4];

        for (int i = 0; i < 4; i++)
        {

            m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
        }

        m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMass;
        m_MaxHandbrakeTorque = float.MaxValue;

        m_Rigidbody = GetComponent<Rigidbody>();
        m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl*m_FullTorqueOverAllWheels);
    }

    private void GearChanging()
    {
        float f = Mathf.Abs(CurrentSpeed/MaxSpeed);
        float upgearlimit = (1/(float) NoOfGears)*(m_GearNum + 1);
        float downgearlimit = (1/(float) NoOfGears)*m_GearNum;

        if (m_GearNum > 0 && f < downgearlimit)
        {
            m_GearNum--;
        }

        if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
        {
            m_GearNum++;
        }
    }

    private static float CurveFactor(float factor)
    {
        return 1 - (1 - factor)*(1 - factor);
    }

    private static float ULerp(float from, float to, float value)
    {
        return (1.0f - value)*from + value*to;
    }

    private void CalculateGearFactor()
    {
        float f = (1/(float) NoOfGears);

        var targetGearFactor = Mathf.InverseLerp(f*m_GearNum, f*(m_GearNum + 1), Mathf.Abs(CurrentSpeed/MaxSpeed));

        m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime*5f);
    }

    private void CalculateRevs()
    {    
        CalculateGearFactor();
        var gearNumFactor = m_GearNum/(float) NoOfGears;
        var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
        var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
        Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
    }

    public void Move(float steering, float accel, float footbrake, float handbrake)
    {
        for (int i = 0; i < 4; i++)
        {
            Quaternion quat;
            Vector3 position;

            m_WheelColliders[i].GetWorldPose(out position, out quat);
            m_WheelMeshes[i].transform.position = position;
            m_WheelMeshes[i].transform.rotation = quat;
        }    
        
        steering = Mathf.Clamp(steering, -1, 1);
        AccelInput = accel = Mathf.Clamp(accel, 0, 1);
        BrakeInput = footbrake = -1*Mathf.Clamp(footbrake, -1, 0);
        handbrake = Mathf.Clamp(handbrake, 0, 1);
    
        m_SteerAngle = steering*m_MaximumSteerAngle;
        m_WheelColliders[0].steerAngle = m_SteerAngle;
        m_WheelColliders[1].steerAngle = m_SteerAngle;

        SteerHelper();
        ApplyDrive(accel, footbrake);
        CapSpeed();

        if (handbrake > 0f)
        {
            var hbTorque = handbrake*m_MaxHandbrakeTorque;
            m_WheelColliders[2].brakeTorque = hbTorque;
            m_WheelColliders[3].brakeTorque = hbTorque;
        }

        CalculateRevs();
        GearChanging();
        AddDownForce();
        TractionControl();
    }


    private void CapSpeed()
    {
        float speed = m_Rigidbody.velocity.magnitude;
        speed *= 3.6f;
        if (speed > m_Topspeed)
            m_Rigidbody.velocity = (m_Topspeed/3.6f) * m_Rigidbody.velocity.normalized;
    }


    private void ApplyDrive(float accel, float footbrake)
    {    
        float thrustTorque = accel * (m_CurrentTorque / 4f);

        for (int i = 0; i < 4; i++)
        {
            m_WheelColliders[i].motorTorque = thrustTorque;
        }

        for (int i = 0; i < 4; i++)
        {
            if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
            {
                m_WheelColliders[i].brakeTorque = m_BrakeTorque*footbrake;
            }
            else if (footbrake > 0)
            {
                m_WheelColliders[i].brakeTorque = 0f;
                m_WheelColliders[i].motorTorque = -m_ReverseTorque*footbrake;
            }
        }
    }

    private void SteerHelper()
    {
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            m_WheelColliders[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return; 
        }

        if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
        }

        m_OldRotation = transform.eulerAngles.y;
    }
    
    private void AddDownForce()
    {
        m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up*m_Downforce * m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
    }

    private void TractionControl()
    {
        WheelHit wheelHit;

        for (int i = 0; i < 4; i++)
        {
            m_WheelColliders[i].GetGroundHit(out wheelHit);

           AdjustTorque(wheelHit.forwardSlip);
        }
    }

    private void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
        {
            m_CurrentTorque -= 10 * m_TractionControl;
        }
        else
        {
            m_CurrentTorque += 10 * m_TractionControl;

            if (m_CurrentTorque > m_FullTorqueOverAllWheels)
            {
               m_CurrentTorque = m_FullTorqueOverAllWheels;
            }
        }

    }
}
