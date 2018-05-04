using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class bulletMovement : MonoBehaviour {

    // Bullet pattern vars
    public float speed = 30;
    public bool bulletImpactCameraShake = false;

    private float amplitute = 0.5f;
    public bool directionRight = true;
    private GameObject player;

    // Animations - Bullet Explosion
    public GameObject collisionAnimation; //Animation Prefab. This animation will play when the bullet collides
    public float animationTime = 2;
    public bool useExplosion = false;

    // Other script access
    public float t_PatternTimer = 0;

    // Delayed "deletion"
    public bool useDelay = false;
    public float time_ForDelay = 0;
    public float t_startDelay = 0;

    // Gun patterns
    public Bullet_Pattern eCurStatePattern; //init to idle
    

    public enum Bullet_Pattern
    {
        none,
        sin,
    }

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update ()
    {

        gameObject.transform.Translate(new Vector2(0, -speed * Time.deltaTime));
        //gameObject.GetComponent<Rigidbody2D>().MovePosition(new Vector2(0, -30 * Time.deltaTime));

        switch(eCurStatePattern)
        {
            case Bullet_Pattern.none:
                
                break;
            case Bullet_Pattern.sin:
                if (directionRight)
                    t_PatternTimer += Time.deltaTime * 8;
                if (!directionRight)
                    t_PatternTimer -= Time.deltaTime * 8;
                
                if (t_PatternTimer >= amplitute)
                    directionRight = false;
                if (t_PatternTimer <= -amplitute)
                    directionRight = true;
                
                    gameObject.transform.Translate(t_PatternTimer, 0, 0);
                
                break;
        }

        if (useDelay)
        {
            t_startDelay += Time.deltaTime;
            if (t_startDelay >= time_ForDelay)
            {
                gameObject.SetActive(false);
                t_startDelay = 0;
            }
        }

    }

    void OnTriggerEnter2D(Collider2D coll)
    {

        if ((coll.gameObject.tag != "Player")&& (coll.gameObject.tag != "Bullet_Player_Regular") && (coll.gameObject.tag != "Bullet_Boss") && (coll.gameObject.tag != "Gun"))
        {
            if ((player.transform.GetChild(player.GetComponent<Inventory>().curGun).GetComponent<GunBehavior>().useCameraShake) && (bulletImpactCameraShake))
                CameraShaker.Instance.ShakeOnce(4f, 4f, .1f, .3f);

            if (useExplosion)
            {
                GameObject ExplosionInstance = Instantiate(collisionAnimation , gameObject.transform.position, gameObject.transform.rotation);
                Destroy(ExplosionInstance, animationTime);
                if((player.transform.position-gameObject.transform.position).magnitude < 4)
                {
                    player.GetComponent<player_Health>().player_HealthBar.value -= (3 * ((player.transform.position - gameObject.transform.position).magnitude ));
                }
            }

                       
            this.gameObject.SetActive(false);
        }

    }
}
