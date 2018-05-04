using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandling : MonoBehaviour {

    // Pooling
    [Header("Pooling")]
    private const int POOL_SIZE = 50; // Shared with all enemyies
    private GameObject[] bulletPool;
    public GameObject bulletOriginal;
    public Transform bulletPoolSpawnPoint;

    // Shooting
    [Header("Shooting")]
    public Transform emitterPos;
    public float recoil = 5;
    public float rateOfFire = 0.1f;


    private float t_shotTimer = 0;

    // Init Bullets
    void Start () {

        bulletPool = new GameObject[POOL_SIZE];

        for (int i = 0; i < POOL_SIZE; i++)
        {
            bulletPool[i] = (GameObject)Instantiate(bulletOriginal, bulletPoolSpawnPoint);
            bulletPool[i].SetActive(false);
        }
    }
	
	void Update () {
		
	}

    void instantiateBullet(float t_betweenShot, Transform emitter, float Angle)
    {

        // If A bullet is not active use that
        for (int i = 0; i < BULLET_POOL_SIZE; i++)
        {
            if (!bulletPool[i].activeInHierarchy)
            {
                // Set guns emitter/sprite to the correct angle/direction with recoil
                emitterPos.transform.rotation = Quaternion.AngleAxis(Angle + 90 + randAngleChange, Vector3.forward);     // Keep Gun + Emitter
               

                bulletPool[i].transform.position = emitter.transform.position;
                bulletPool[i].transform.rotation = emitter.transform.rotation;
                bulletPool[i].GetComponent<bulletMovement>().t_PatternTimer = 0;
                bulletPool[i].GetComponent<bulletMovement>().directionRight = false;
                bulletPool[i].SetActive(true);

                
                //shotSound.Play();

                break;
            }
        }
        // Reset timer
        shotTimer = 0;
        
    }
}
