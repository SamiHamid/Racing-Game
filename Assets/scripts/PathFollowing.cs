using System;
using UnityEngine;

  public class PathFollowing : MonoBehaviour
  {
        public WaypointsContainer waypoints; 
        public float lookAheadForTargetOffset = 20;
        public float lookAheadForTargetFactor = .1f;
        public float lookAheadForSpeedOffset = 1;
        public float lookAheadForSpeedFactor = .05f;
        
        public WaypointsContainer.RoutePoint targetPoint { get; private set; }
        public WaypointsContainer.RoutePoint speedPoint { get; private set; }
        public WaypointsContainer.RoutePoint progressPoint { get; private set; }

        public Transform target;

        private float progressDistance; 
        private int progressNum;
        private Vector3 lastPosition; 
        private float speed; 

        
        private void Start()
        {
            if (target == null)
            {
                target = new GameObject(name + " Waypoint Target").transform;
            }

            Reset();
        }

        public void Reset()
        {
            progressDistance = 0;
            progressNum = 0;
        }


        private void Update()
        {
                if (Time.deltaTime > 0)
                {
                    speed = Mathf.Lerp(speed, (lastPosition - transform.position).magnitude/Time.deltaTime,
                                       Time.deltaTime);
                }

                target.position = waypoints.GetRoutePoint(progressDistance + lookAheadForTargetOffset + lookAheadForTargetFactor*speed).position;

                 target.rotation = Quaternion.LookRotation(waypoints.GetRoutePoint(progressDistance + lookAheadForSpeedOffset + lookAheadForSpeedFactor*speed).direction);

                progressPoint = waypoints.GetRoutePoint(progressDistance);

                Vector3 progressDelta = progressPoint.position - transform.position;

                if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
                {
                    progressDistance += progressDelta.magnitude*0.5f;
                }

                lastPosition = transform.position;
            
        }
    
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, target.position);
                Gizmos.DrawWireSphere(waypoints.GetRoutePosition(progressDistance), 1);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(target.position, target.position + target.forward);
            }
        }
}
