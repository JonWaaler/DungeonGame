using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class bulletMovement : MonoBehaviour {

    // Bullet vars
    [Header("Bullet Movement")]
    public float speed = 30;
    public float amplitute = 0.5f;
    // Public for other script access
    public bool directionRight = true;
    private GameObject player;

    [Header("Collision Sound")]
    public GameObject OnCollisionSound;
    public Transform parent;

    [Header("Camera Shake")]
    public bool bulletImpactCameraShake = false;

    [Header("Collision Ignore Tags")]
    public List<string> collIgnoreTags;

    // Animations - Bullet Explosion
    [Header("Collision Animations")]
    public bool Anim_BulletDeath = false;
    public GameObject collisionAnimation; //Animation Prefab. This animation will play when the bullet collides
    public float animationTime = 2;

    // Delayed "deletion"
    [Header("Delayed Deletion")]
    public bool useDelay = false;
    public float time_ForDelay = 0;

    // Gun patterns
    [Header("Bullet Pattern")]
    public Bullet_Pattern eCurStatePattern; //init to idle

    // Timers
    [Header("Timer (Don't Change)")]
    public float t_startDelay = 0;
    public float t_PatternTimer = 0;

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

        switch (eCurStatePattern)
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
        // Ignore Tag for collision
        bool ignorePassed = false;
        int counter = 0;
        for (int i = 0; i < collIgnoreTags.Capacity; i++)
        {
            if(coll.gameObject.tag != collIgnoreTags[i])
            {
                counter++;
            }
        }

        if (counter == collIgnoreTags.Capacity)
            ignorePassed = true;

        // If all tag i want to ignore are true, then it's something i want to collide w/
        if (ignorePassed)
        {
            // Not the best way of doing it, however ran into problems where the previous
            // Effitient  method stoped working
            if (OnCollisionSound != null)
            {
                GameObject temp;
                Instantiate(OnCollisionSound);
                temp = (GameObject)Instantiate(OnCollisionSound, parent);

                Destroy(temp, 1);
            }

            if ((player.transform.GetChild(player.GetComponent<Inventory>().curGun).GetChild(0).GetComponent<GunBehavior>().useCameraShake) && (bulletImpactCameraShake))
                CameraShaker.Instance.ShakeOnce(10f, 8f, 0.1f, .8f);

            if (Anim_BulletDeath)
            {
                //print("Make Boom");
                GameObject ExplosionInstance = Instantiate(collisionAnimation, gameObject.transform.position, gameObject.transform.rotation);
                Destroy(ExplosionInstance, animationTime);
                if ((player.transform.position - gameObject.transform.position).magnitude < 4)
                {
                    player.GetComponent<player_Health>().player_HealthBar.value -= (3 * ((player.transform.position - gameObject.transform.position).magnitude));
                }
            }

            gameObject.SetActive(false);
        }
    }
}
