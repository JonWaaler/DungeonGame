using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShooting : MonoBehaviour {

    public GameObject emitter;
    public GameObject boss;
    public GameObject bulletPrefab;
    private float angle = 0;
    private float timeCount = 0;

    void Update()
    {
        gameObject.transform.Translate(new Vector2(0, -10 * Time.deltaTime));
        angle += Time.deltaTime;

        emitter.transform.rotation = Quaternion.AngleAxis(angle , Vector3.forward);

        GameObject bulletInstance;

        bulletInstance = Instantiate(bulletPrefab, emitter.transform.position, emitter.transform.rotation) as GameObject;
        
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        print("Boss Working");
        //damage player
    }
}
