using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

/* public var = Audio_MySound;
 * private vars = audio_MySound;
 * 
 */
public class GunBehavior : MonoBehaviour
{
    // GUN Data
    [Header("Audio Options")]
    public AudioSource Audio_shot;
    public AudioSource Audio_reload;
    public AudioSource Audio_emptyMag;

    [Header("Gun Firing Options")]
    public float data_shotRate;                 //How fast you shoot. Time between shot
    public float data_recoil;                   //after every shot random range of (recoil,recoil) 
    public float data_damage;
    public bool useCameraShake;


    [Header("Gun Animations")]
    public GameObject anim_GunWarmUp;

    public GameObject Anim_ShootPuff;
    GameObject anim_puff;

    // Lightning patterns
    [Header("Effects")]
    public Gun_Types eCurGunState; //init to idle
    public ParticleSystem lightningParticles;

    public enum Gun_Types
    {
        regular,
        Lightning,
    }

    //public Sprite[] randomSpriteShots;
    public List<Sprite> random_BulletSprites;

    // Reloading set up
    [Header("Reloading Set-up")]
    public GameObject throwShells;
    public float reloadTime;
    public int magCapacity;
    public int totalBullets;
    public bool enableSpriteRecoil = true;
    private int amtInMag;
    private float t_reload = 0;
    private bool startReload;
    private bool hasPlayed_ThrowShells = false;

    // Gun/emitter placement
    [Header("Emitter/Sprite Placement")]
    public Transform emitterPosition;
    public float EmitX, EmitY;
    public float SpriteX, SpriteY;

    // Instantiate Set-up
    [Header("Instantiate Bullet Set-up")]
    public GameObject player;
    public GameObject bulletOriginal;
    private GameObject gunSprite;
    private Transform player_emitter;
    
    // Pooling
    [Header("Pooling")]
    public const int BULLET_POOL_SIZE = 25;
    public Transform bulletPoolSpawnPoint;
    public GameObject[] bulletPool;

    // Gun set-up
    [Header("Public just for Outside scripts")]
    public bool isShooting = false;        // Initialize player to not shooting.
    private float randAngleChange = 0;      // The angle amt at which the gun will recoil
    private float shotTimer = 0;            // Initialize the timer to start counting at 0. This counts up to timeBetweenShot

    // Start Init Invtory
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        bulletPoolSpawnPoint = GameObject.Find("PlayerBulletPool").transform;

        bulletPool = new GameObject[BULLET_POOL_SIZE];
        amtInMag = magCapacity;

        // Bullet Pooling Spawn
        for (int i = 0; i < BULLET_POOL_SIZE; i++)
        {
            bulletPool[i] = (GameObject)Instantiate(bulletOriginal, bulletPoolSpawnPoint);
            bulletPool[i].SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        if (gameObject.transform.parent.parent != null)
        {
            gameObject.transform.parent.localPosition = new Vector3(0, 0, 0);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;

            player = GameObject.FindGameObjectWithTag("Player");

            //gameObject.GetComponent<GunBehavior>().enabled = false;

            //int curGun = 0;

            // Our currently selected gun
            //int curGun = gameObject.GetComponentInParent<Inventory>().curGun;
            int curGun = gameObject.transform.parent.parent.GetComponent<Inventory>().curGun;



            // Variable Initializers (Reduce GetComponent Calls)
            // Set-up for Gun data to change how we shoot
            gunSprite = player.transform.GetChild(curGun).GetChild(0).gameObject;


            // For angle math
            Vector3 pos = Camera.main.WorldToScreenPoint(player.transform.GetChild(curGun).GetChild(0).position);   // Mouse to world Pos
            Vector3 dir = Input.mousePosition - pos;                                                    // Direction vector

            // Angle math used to set up where emmiter looks
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;                                    // Angle Calculation

            // Recoil to be applied to angle math
            randAngleChange = Random.Range(-data_recoil, data_recoil);

            // Too make sure we don't recoil the gun  when not shooting
            if (isShooting == false)
                randAngleChange = 0;

            // Set guns emitter/sprite to the correct angle/direction without recoil
            emitterPosition.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);     // Keep Gun + Emitter
            gunSprite.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);          // Looking in correct dir

            // Increment shot timer
            // Shot timer counts up till: shotRate. Then allows us to shoot
            shotTimer += Time.deltaTime;                                                                            // Timer Update

            // We need to compare our mouse in world position vs player
            // This is because camera isn't static
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //--- Player sprite flip / Gun layer switch ---//
            if (mousePos.x >= player.transform.position.x)
            {   //mouse - | RIGHT |- of player
                emitterPosition.localPosition = new Vector3(EmitX, EmitY, 0);
                gunSprite.transform.localPosition = new Vector3(SpriteX, -SpriteY, 0);
                gunSprite.GetComponent<SpriteRenderer>().flipY = false; //flips gun depending on left/right side of player

                lightningParticles.transform.localPosition = new Vector3(0.127f, 0.203f, 2.03f);
            }
            else if (mousePos.x < player.transform.position.x)
            {   //mouse -| LEFT |- of player
                emitterPosition.localPosition = new Vector3(EmitX, -EmitY, 0);
                gunSprite.transform.localPosition = new Vector3(-SpriteX, -SpriteY, 0);
                gunSprite.GetComponent<SpriteRenderer>().flipY = true; //flips gun depending on left/right side of player
                
                lightningParticles.transform.localPosition = new Vector3(0.127f, -0.203f, 2.03f);
            }

            if ((amtInMag == 0) && (Input.GetMouseButtonDown(0))&&(!isShooting))
            {
                startReload = true;
                hasPlayed_ThrowShells = false;
            }

            // Mouse hold down shooting
            if (Input.GetMouseButtonDown(0))
                isShooting = true;
            if (Input.GetMouseButtonUp(0))
                isShooting = false;

            // Reloading
            if ((Input.GetKeyDown(KeyCode.R)) && (!isShooting))
            {
                // Reload

                if (totalBullets > 0)
                {
                    hasPlayed_ThrowShells = false;
                    startReload = true;
                }
                else
                    Audio_emptyMag.Play();
            }

            if (startReload)
            {
                t_reload += Time.deltaTime;
                if (t_reload >= reloadTime)
                {
                    // Only Plays if you actually need to reload
                    if (amtInMag != magCapacity)
                    {
                        if (!hasPlayed_ThrowShells)
                        {

                            Audio_reload.Play();
                            GameObject anim = Instantiate(throwShells);
                            anim.transform.position = gameObject.transform.position;

                            if (gameObject.transform.position.x < player.transform.position.x)
                                anim.GetComponent<SpriteRenderer>().flipX = true;
                            else
                                anim.GetComponent<SpriteRenderer>().flipX = false;

                            hasPlayed_ThrowShells = true;
                        }
                    }

                    // Always plays, even if amtInMag == magCapacity
                    if ((totalBullets - magCapacity) >= magCapacity)
                        amtInMag = magCapacity;
                    else
                        amtInMag = totalBullets;


                    startReload = false;
                                        
                    t_reload = 0;
                }
            }
            lightningParticles.Stop();
            // Shooting instantiate
            if (amtInMag > 0)
            {
                if (eCurGunState == Gun_Types.regular)
                    instantiateBullet(data_shotRate, emitterPosition, angle);
                else if (eCurGunState == Gun_Types.regular)
                {
                    lightningParticles.Play();
                }
            }
            else if ((isShooting) && (!Audio_emptyMag.isPlaying) && (!startReload))
                Audio_emptyMag.Play();

            if (anim_puff != null)
            {
                anim_puff.transform.position = gameObject.transform.GetChild(0).position;
                anim_puff.transform.rotation = gameObject.transform.GetChild(0).rotation;
                anim_puff.transform.Rotate(0, 0, -90);
            }


            if (isShooting)
                lightningParticles.Play();
        }
        else // No parent meaning on the ground
        {
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = -5;
        }
        Audio_shot.pitch = Random.Range(0.8f, 1.2f);
    }

    void instantiateBullet(float t_betweenShot, Transform emitter, float Angle)
    {

        if ((isShooting) && (shotTimer >= t_betweenShot) && (startReload == false))
        {
            // If A bullet is not active use that
            for (int i = 0; i < BULLET_POOL_SIZE; i++)
            {
                if (!bulletPool[i].activeInHierarchy)
                {
                    // Set guns emitter/sprite to the correct angle/direction with recoil
                    emitterPosition.transform.rotation = Quaternion.AngleAxis(Angle + 90 + randAngleChange, Vector3.forward);     // Keep Gun + Emitter
                    if(enableSpriteRecoil)
                        gunSprite.transform.rotation = Quaternion.AngleAxis(Angle + randAngleChange, Vector3.forward);          // Looking in correct dir
                    amtInMag -= 1; // Decrement Bullets in mag
                    totalBullets -= 1;
                    bulletPool[i].GetComponent<bulletMovement>().t_PatternTimer = 0;
                    bulletPool[i].transform.position = emitter.transform.position;
                    bulletPool[i].transform.rotation = emitter.transform.rotation;
                    bulletPool[i].GetComponent<bulletMovement>().t_PatternTimer = 0;
                    bulletPool[i].GetComponent<bulletMovement>().directionRight = false;
                    bulletPool[i].SetActive(true);

                    if(useCameraShake)
                        CameraShaker.Instance.ShakeOnce(4f, 4f, .1f, .3f);

                    int spritePick = Random.Range(0, random_BulletSprites.Capacity);
                    bulletPool[i].GetComponent<SpriteRenderer>().sprite = random_BulletSprites[spritePick];

                    // Shoot puff/smoke animation
                    if(anim_puff  != null)
                    {
                        anim_puff = Instantiate(Anim_ShootPuff);
                        anim_puff.transform.position = gameObject.transform.GetChild(0).position;
                        anim_puff.transform.rotation = gameObject.transform.GetChild(0).rotation;
                        Destroy(anim_puff, 0.5f);
                    }
                    
                    Audio_shot.Play();

                    break;
                }
            }
            // Reset timer
            shotTimer = 0;
        }
    }
}

