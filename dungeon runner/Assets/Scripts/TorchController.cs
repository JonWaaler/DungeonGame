using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchController : MonoBehaviour {
    public Light myLight;
    private float prevTime = 0;
	
	void Update () {
        if (prevTime <= (Time.time - Random.Range(0.08f, 0.15f)))
        {
            // Access touch intensity and rodomise it
            myLight.intensity = Random.Range(3.0f, 9.0f);
            prevTime = Time.time;
        }        
	}
}
