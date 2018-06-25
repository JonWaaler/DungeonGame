using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandling : MonoBehaviour {
    //Copy  Paste from boss, changed to suite Enemys

    // Behavior Break down
    /* - Raycast check for player
     * - if raycast return false, walk to player
     * - else, start shooting
     * 
     * Harder enemyies have more patterns
     * 
     * 
     * Raycast dir to player
     * if(hit == player) || (hit == breakables) then run shoot()
     * else walk nav mesh to player
     */

    public GameObject bulletOrginal;        //Bullet prefab selected
    private GameObject enemy_TransformLayer; //This should be used for moving the boss around. When you rotate "boss_SpriteHolder" it wont affect this
    private GameObject enemy_SpriteLayer;
    private GameObject enemy_Gun;        //This is the bullets initialize point
    private GameObject enemy_Emitter;
    private GameObject player;              //The Player
    private float enemyHealth = 7;

    // Shooting Variables
    [Header("Shooting Variables")]
    private float timer_BetweenShot = 0;
    private float timer_BeforeReload = 0;
    private float timer_wait = 0;
    public float timeBetweenShot = 0.05f;
    public float timeBeforeReload = 5;
    public float waitTime = 5;

    // Follow
    [Header("Follow")]
    public bool maintainDistance;
    public float distance;


        
    // Boss bullet pool
    private GameObject[] enemy_BulletPool;       // Array of bullets
    private const int BULLET_POOL_SIZE = 30;    // Boss bullet pool size
    public Transform enemy_BulletSpawn;

    public float rotSpeed;
    public float recoil;
    public float speed;

    private Vector3 enemy_GunPos;

    void Start()
    {
        enemy_TransformLayer = gameObject.transform.parent.gameObject;
        enemy_SpriteLayer = gameObject;
        enemy_Gun = gameObject.transform.GetChild(0).gameObject;
        enemy_Emitter = enemy_Gun.transform.GetChild(0).gameObject;

        enemy_GunPos = enemy_Gun.transform.localPosition;

        player = GameObject.Find("Player");

        enemy_BulletPool = new GameObject[BULLET_POOL_SIZE];

        // Bullet Pooling Spawn
        for (int i = 0; i < BULLET_POOL_SIZE; i++)
        {
            enemy_BulletPool[i] = (GameObject)Instantiate(bulletOrginal, enemy_BulletSpawn);
            enemy_BulletPool[i].SetActive(false);
        }
    }

    void Update()
    {
        HandleShootingState();
        FollowPlayer(maintainDistance, distance);
        
        // Move the gun on the correct side of the enemy
        if(player.transform.position.x >= enemy_TransformLayer.transform.position.x)
        {
            enemy_Gun.transform.localPosition = new Vector3(enemy_GunPos.x, enemy_GunPos.y, enemy_GunPos.z);
        }
        else
        {
            enemy_Gun.transform.localPosition = new Vector3(-enemy_GunPos.x, enemy_GunPos.y, enemy_GunPos.z);
        }
    }

    float LookAt(GameObject target, GameObject entity, float offset)
    {
        float AngleRad = Mathf.Atan2(target.transform.position.y - entity.transform.position.y, target.transform.position.x - entity.transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;

        return AngleDeg + offset;
    }

    void HandleShootingState()
    {
        //Aim Lock on player           
        //enemy_SpriteLayer.transform.rotation = Quaternion.Euler(0, 0, LookAt(player, enemy_SpriteLayer, 90));
        enemy_Gun.transform.rotation = Quaternion.Euler(0, 0, LookAt(player, enemy_Emitter, 90));
        
        
        shooting(timeBetweenShot, timeBeforeReload);        
    }

    // FollowPlayer
    void FollowPlayer(bool ans, float distance)
    {
        Vector3 distVector = enemy_TransformLayer.transform.position - player.transform.position;
        float dist = distVector.magnitude;

        if (dist > distance) //go closer to player i dist > distance
        {
            Vector3 speed = distVector.normalized;

            enemy_TransformLayer.transform.Translate(-speed * Time.deltaTime * 5);
        }
    }

    //Shooting Definition
    void shooting(float timeBetweenShotLC, float timeBeforeReloadLC) //LC = local, needed differentiation from pub vars
    {
        timer_BeforeReload += Time.deltaTime;
        timer_BetweenShot += Time.deltaTime;

        //Count up timer, until timer is greater than the desired "timeBeforeReload"
        if (timer_BeforeReload <= timeBeforeReload)
        {
            if (timer_BetweenShot >= timeBetweenShotLC)
            {
                //GameObject bulletInstance;
                //bulletInstance = Instantiate(bulletOrginal, boss_Emitter.transform.position, boss_Emitter.transform.rotation) as GameObject;
                for (int i = 0; i < BULLET_POOL_SIZE; i++)
                {
                    if (!enemy_BulletPool[i].activeInHierarchy)
                    {
                        enemy_BulletPool[i].transform.position = enemy_Emitter.transform.position;
                        enemy_BulletPool[i].transform.rotation = enemy_Emitter.transform.rotation;
                        enemy_BulletPool[i].SetActive(true);

                        break;
                    }
                }


                timer_BetweenShot = 0;
            }
        }
        if (timer_BeforeReload >= timeBeforeReload)
        {
            timer_wait += Time.deltaTime;

            if (timer_wait >= waitTime)
            {
                timer_BeforeReload = 0;
                timer_wait = 0;
            }
        }
    }

    // Bullet Collisions
    void OnTriggerEnter2D(Collider2D coll)
    {

        if ((coll.gameObject.tag == "Bullet_Player_Regular") && (coll.gameObject.tag != "Boss") && (coll.gameObject.tag != "Bullet_Boss"))
        {
            //Health Bar go down
            enemyHealth -= player.transform.GetChild(player.GetComponent<Inventory>().curGun).GetChild(0).GetComponent<GunBehavior>().data_damage;
        }
        if (enemyHealth <= 0)
        {
            Destroy(enemy_TransformLayer);
        }
    }
}
