using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof (CarController))]
public class OpponentController : MonoBehaviour
{
    [Range(0, 1)] public float m_CautiousSpeedFactor = 0.5f;              
    [Range(0, 180)] public float m_CautiousMaxAngle = 50f;                  
    public float m_CautiousMaxDistance = 20f;                              
    public float m_CautiousAngularVelocityFactor = 5f;                     
    public float m_SteerSensitivity = 1f;                                
    public float m_AccelSensitivity = 1f;                                
    public float m_BrakeSensitivity = 1f;                                   
    public float m_LateralWanderDistance = 0.5f;                              
    public float m_LateralWanderSpeed = 0.01f;                               
    [Range(0, 1)] public float m_AccelWanderAmount = 0.1f;                  
    public float m_AccelWanderSpeed = 0.005f;                                 
    public bool m_Driving;                                                  
    public Transform m_Target;                                                                                                                                          

    private float m_RandomPerlin;            
    private CarController m_CarController;   
    private float m_AvoidOtherCarTime;       
    private float m_AvoidOtherCarSlowdown;   
    private float m_AvoidPathOffset;                                    
    private Rigidbody m_Rigidbody;

    private void Awake()
    {
        m_CarController = GetComponent<CarController>();
        m_RandomPerlin = Random.value*100;
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (m_Target == null || !m_Driving)
        {
            m_CarController.Move(0, 0, -1f, 1f);
        }

        else
        {
            Vector3 fwd = transform.forward;

            if (m_Rigidbody.velocity.magnitude > m_CarController.MaxSpeed * 0.1f)
            {
                fwd = m_Rigidbody.velocity;
            }

            float desiredSpeed = m_CarController.MaxSpeed;
            float approachingCornerAngle = Vector3.Angle(m_Target.forward, fwd);
            float spinningAngle = m_Rigidbody.angularVelocity.magnitude*m_CautiousAngularVelocityFactor;
            float cautiousnessRequired = Mathf.InverseLerp(0, m_CautiousMaxAngle, Mathf.Max(spinningAngle, approachingCornerAngle));

            desiredSpeed = Mathf.Lerp(m_CarController.MaxSpeed, m_CarController.MaxSpeed*m_CautiousSpeedFactor, cautiousnessRequired);

            Vector3 offsetTargetPos = m_Target.position;

            if (Time.time < m_AvoidOtherCarTime)
            {
                desiredSpeed *= m_AvoidOtherCarSlowdown;

                offsetTargetPos += m_Target.right*m_AvoidPathOffset;
            }
            else
            {
                offsetTargetPos += m_Target.right* (Mathf.PerlinNoise(Time.time*m_LateralWanderSpeed, m_RandomPerlin)*2 - 1) * m_LateralWanderDistance;
            }

            float accelBrakeSensitivity = (desiredSpeed < m_CarController.CurrentSpeed) ? m_BrakeSensitivity : m_AccelSensitivity;
            float accel = Mathf.Clamp((desiredSpeed - m_CarController.CurrentSpeed)*accelBrakeSensitivity, -1, 1);

            accel *= (1 - m_AccelWanderAmount) + (Mathf.PerlinNoise(Time.time*m_AccelWanderSpeed, m_RandomPerlin)*m_AccelWanderAmount);

            Vector3 localTarget = transform.InverseTransformPoint(offsetTargetPos);

            float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z)*Mathf.Rad2Deg;
            float steer = Mathf.Clamp(targetAngle*m_SteerSensitivity, -1, 1)*Mathf.Sign(m_CarController.CurrentSpeed);

            m_CarController.Move(steer, accel, accel, 0f);
        }
    }

    private void OnCollisionStay(Collision col)
    {    
        if (col.rigidbody != null)
        {
            var otherAI = col.rigidbody.GetComponent<OpponentController>();
          
            if (otherAI != null)
            {
                    m_AvoidOtherCarTime = Time.time + 1;

                    if (Vector3.Angle(transform.forward, otherAI.transform.position - transform.position) < 90)
                    {
                        m_AvoidOtherCarSlowdown = 0.5f;
                    }
                    else
                    {
                        m_AvoidOtherCarSlowdown = 1;
                    }

                    var otherCarLocalDelta = transform.InverseTransformPoint(otherAI.transform.position);
                    float otherCarAngle = Mathf.Atan2(otherCarLocalDelta.x, otherCarLocalDelta.z);
                    m_AvoidPathOffset = m_LateralWanderDistance*-Mathf.Sign(otherCarAngle);
            }
        }
    }

    public void SetTarget(Transform target)
    {
        m_Target = target;
        m_Driving = true;
    }
}
