using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPosition : MonoBehaviour {
    public Transform attachTo;
    public bool attachRot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.position = new Vector2( attachTo.position.x, attachTo.position.y);


        if (attachRot)
            gameObject.transform.rotation = attachTo.rotation;

    }
}
