using System;
using UnityEngine;

   [RequireComponent(typeof (CarController))]
    public class PlayerController : MonoBehaviour
    {
        private CarController carController; 

        private void Awake()
        {
            carController = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float handbrake = Input.GetAxis("Jump");
            carController.Move(h, v, v, handbrake);
        }
}
