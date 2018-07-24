using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;



public class playerMovement : MonoBehaviour {
    public float speed;
    private float xySpeed;
    public GameObject crosshair;
    private Rigidbody2D player;
    private Transform player_emitter;
    public Sprite Player_front;
    public Sprite Player_back;
    public Sprite Player_frontside;
    public Sprite Player_backside;
    public Sprite Player_dodge;
    private SpriteRenderer playerSprite;

    // Dodge Timer
    float t_dodgeDelay = 2;
    float t_dodgeInterp = 0;
    Vector2 newPosition = new Vector2(0, 0);
    bool triggerBoost = false;
    public LayerMask myLayerMask;
    

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        playerSprite = player.GetComponent<SpriteRenderer>();
        xySpeed = Mathf.Sqrt(Mathf.Pow(speed, 2) / 2.0f);
    }

    void FixedUpdate()
    {
        //player_emitter = GameObject.Find("Emitter").GetComponent<Transform>();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        crosshair.transform.position = mousePos;
        

        //--- Player sprite flip / Gun layer switch ---//
        if (mousePos.x > player.position.x)
        {   //mouse right of player
            playerSprite.flipX = true;
        }
        if (mousePos.x < player.position.x)
        {   //mouse left of player
            playerSprite.flipX = false;
        }

        if(mousePos.y > player.position.y)
            playerSprite.sortingOrder = 4;
        
        if (mousePos.y < player.position.y)
            playerSprite.sortingOrder = -4;

        // Player - Mouse angle
        Vector3 pos = Camera.main.WorldToScreenPoint(player.transform.GetChild(player.GetComponent<Inventory>().curGun).position);   // Mouse to world Pos
        Vector3 dir = Input.mousePosition - pos;                                                    // Direction vector

        // Mouse angle
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;                                    // Angle Calculation


        //--- Player direction animation ---//
        // Top View
        if ((angle >= 0) && (angle < 67.5))
        {
            playerSprite.sprite = Player_backside;
        }
        if ((angle >= 67.5) && (angle < 90))
        {
            playerSprite.sprite = Player_back;
        }
        if ((angle >= 90) && (angle < 112.5))
        {
            playerSprite.sprite = Player_backside;
        }
        if ((angle >= 112.5) && (angle < 180))
        {
            playerSprite.sprite = Player_backside;
        }

        // Bottom View
        if ((angle <= 0) && (angle > -67.5))
        {
            playerSprite.sprite = Player_frontside;
        }        
        if ((angle <= -67.5) && (angle > -90))
        { 
            playerSprite.sprite = Player_front;
        }        
        if ((angle <= -90) && (angle >= -112.5))
        { 
            playerSprite.sprite = Player_front;
        }
        if ((angle <= -112.5) && (angle >= -180))
        {
            playerSprite.sprite = Player_frontside;
        }

        // Movement Variables
        float x, y;
        x = 0;
        y = 0;

        // Boost
        float boostAmount = 12.0f;

        // Input to change movement vars
        if (Input.GetButton("Up"))
            y = 1;
        
        if (Input.GetButton("Down"))
            y = -1;
        
        if (Input.GetButton("Left"))
            x = -1;
        
        if (Input.GetButton("Right"))
            x = 1;



        if(t_dodgeDelay >= 0.5f) // if timer > 2, then allow dodge
        {
            if (Input.GetMouseButtonDown(1))
            {
                if ((x != 0) || (y != 0))
                {
                    //This is to keep consistent speed
                    // If were going diagonally.. adjust the speed
                    if ((x != 0) && (y != 0))
                        boostAmount = Mathf.Sqrt(Mathf.Pow(12,2)/2.0f);
                    else // Else use same speed
                        boostAmount = 12;

                    triggerBoost = true;
                    t_dodgeDelay = 0;

                    // Raycast to see where how close we are to a collision
                    RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(player.transform.position.x, player.transform.position.y), new Vector2(x, y));
                    // Our new position in space
                    newPosition = new Vector2(x * boostAmount + player.transform.position.x, y * boostAmount + player.transform.position.y);

                    // Compare vector lengths, then set position if the ray cast is smaller
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if(hits[i].collider.tag == "Wall")
                        {
                            //newPosition = hits[i].point;
                            if (newPosition.magnitude >= hits[i].distance)
                                newPosition = hits[i].point;

                            break;
                        }
                    }     
                    
                }
            }
        }
        // Increment our delay
        t_dodgeDelay += Time.deltaTime;



        // Players Boost movement translation
        if (triggerBoost)
        {
            Vector2 lerpPos = Vector2.Lerp(player.transform.position, newPosition, t_dodgeInterp);
            player.transform.position = lerpPos;
            t_dodgeInterp += Time.deltaTime*6f;
            playerSprite.sprite = Player_dodge;
        }
        // Player Regular movement translation
        else
        {
            // So there is no speed up when going diagonal
            if ((x != 0) && (y != 0))
                player.velocity = new Vector2(x * Time.deltaTime * xySpeed, y * Time.deltaTime * xySpeed);
            else
                player.velocity = new Vector2(x * Time.deltaTime * speed, y * Time.deltaTime * speed);
        }

        // When Lerp finished, Reset everything
        if (t_dodgeInterp>=0.9f)
        {
            triggerBoost = false;
            t_dodgeInterp = 0;
            newPosition = new Vector2(0, 0);
            t_dodgeDelay = 0.95f;
        }
    }
}