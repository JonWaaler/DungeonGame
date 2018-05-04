using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossShooting : MonoBehaviour {
    public GameObject bulletOrginal;        //Bullet prefab selected
    private GameObject boss_TransformLayer; //This should be used for moving the boss around. When you rotate "boss_SpriteHolder" it wont affect this
    private GameObject boss_SpriteHolder;   //This is used for animations, and and visual effects  or bullet patterns
    private GameObject boss_Emitter;        //This is the bullets initialize point
    private GameObject player;              //The Player

    // Shooting Variables
    private float timer_BetweenShot = 0;
    private float timer_BeforeReload = 0;
    private float timer_wait = 0;
    public float timeBetweenShot = 0.05f;
    public float timeBeforeReload = 5;
    public float waitTime = 5;

    // Follow
    public bool maintainDistance;
    public float distance;

    // Timer for Random state every (sec)
    public float timer_RandomState;
     

    // New boss code
    private BossActionType eCurStateBoss = BossActionType.Follow; //init to idle
    public Slider bossHealth;

    // Boss bullet pool
    private GameObject[] boss_BulletPool;       // Array of bullets
    private const int BULLET_POOL_SIZE = 30;    // Boss bullet pool size
    public Transform boss_BulletSpawn;


    public enum BossActionType
    {
        Idle,
        Moving,
        Follow,
        Patrolling,
        Shooting
    }

    void Start()
    {
        boss_TransformLayer = GameObject.Find("Boss_TransformLayer");
        boss_SpriteHolder = boss_TransformLayer.transform.GetChild(0).gameObject;
        boss_Emitter = boss_SpriteHolder.transform.GetChild(0).gameObject;
        player = GameObject.Find("Player");

        boss_BulletPool = new GameObject[BULLET_POOL_SIZE];

        // Bullet Pooling Spawn
        for (int i = 0; i < BULLET_POOL_SIZE; i++)
        {
            boss_BulletPool[i] = (GameObject)Instantiate(bulletOrginal, boss_BulletSpawn);
            boss_BulletPool[i].SetActive(false);
        }

        timer_RandomState = 0;
    }
    
    void Update()
    {
        timer_RandomState += Time.deltaTime;

        if (timer_RandomState > 10)
        {
            int numState = Random.Range(1, 5);


            if (numState == 1)
                eCurStateBoss = BossActionType.Idle;
            if (numState == 2)
                eCurStateBoss = BossActionType.Moving;
            if (numState == 3)
                eCurStateBoss = BossActionType.Patrolling;
            if (numState == 4)
                eCurStateBoss = BossActionType.Follow;
            if (numState == 5)
                eCurStateBoss = BossActionType.Shooting;

            timer_RandomState = 0;
        }

        eCurStateBoss = BossActionType.Shooting;

        switch (eCurStateBoss)
        {
            case BossActionType.Idle:
                HandleIdleState();
                break;

            case BossActionType.Moving:
                HandleMovingState();
                break;

            case BossActionType.Patrolling:
                HandlePatrollingState();
                break;

            case BossActionType.Follow:
                HandleFollowState();
                break;

            case BossActionType.Shooting:
                HandleShootingState();
                break;
        }
    }

    // State Handling
    void HandleIdleState()
    {
        print("idle");
    }

    void HandleMovingState()
    {
        print("move");
    }

    void HandlePatrollingState()
    {
        // make a defined path for the boss to move on
        print("patroll");
    }

    void HandleFollowState()
    {
        FollowPlayer(maintainDistance, distance);
    }

    void HandleShootingState()
    {
        shooting(timeBetweenShot, timeBeforeReload);
    }

    // FollowPlayer
    void FollowPlayer(bool ans, float distance)
    {
        Vector3 distVector = boss_TransformLayer.transform.position - player.transform.position;
        float dist = distVector.magnitude;

        if (dist > distance) //go closer to player i dist > distance
        {
            Vector3 speed = distVector.normalized;

            boss_TransformLayer.transform.Translate(-speed * Time.deltaTime * 5);
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
                    if (!boss_BulletPool[i].activeInHierarchy)
                    {
                        boss_BulletPool[i].transform.position = boss_Emitter.transform.position;
                        boss_BulletPool[i].transform.rotation = boss_Emitter.transform.rotation;
                        boss_BulletPool[i].SetActive(true);

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
            bossHealth.value -= player.transform.GetChild(player.GetComponent<Inventory>().curGun).GetComponent<GunBehavior>().damage;
        }
        if (bossHealth.value <= 0)
        {
            Destroy(boss_TransformLayer);
        }            
    }
}
