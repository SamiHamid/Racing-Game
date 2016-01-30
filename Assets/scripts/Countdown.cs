using UnityEngine;
using System.Collections;

public class Countdown : MonoBehaviour
{ 
    public static Countdown instance;

    public GameObject[] countdownArray;

    void Awake()
    {
        instance = this;
       // StartCountdown();
    }
	
    
}
