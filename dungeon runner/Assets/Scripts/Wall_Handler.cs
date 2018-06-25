using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Handler : MonoBehaviour {

    public GameObject OnCollisionSound;
    public Transform parent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.tag == "Player_Bullet_Regular")
        {
            print("Col W/ Bullet");
            Instantiate(OnCollisionSound);
            var temp = (GameObject)Instantiate(OnCollisionSound, parent);

           //if (OnCollisionSound.GetComponent<.isPlaying == false)
           //    Destroy(this);

        }
    }
}
