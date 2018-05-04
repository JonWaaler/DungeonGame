using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    // Inventory Set-up
    public GameObject[] guns;
    public int curGun = 0; // curGun = the players selected gun
    public const int INV_SIZE = 5;
    
    // Variables for Gun Behavior Script access
    public GameObject currentGunTransform;
    public GameObject currentGunSprite;

    // player GameObj
    private GameObject player;
    int closestGun = 0;

    void Start () {
        player = GameObject.Find("Player");
        guns = new GameObject[INV_SIZE];

        // For all children (Only Weapons should be attached to player)
        for (int i = 0; i < player.transform.childCount; i++)
        {
            // Delete last guns/items if more than 5 guns/items
            if (player.transform.childCount >= INV_SIZE)
                for (int itr = INV_SIZE; itr < player.transform.childCount; itr++)
                    Destroy(player.transform.GetChild(itr).gameObject);
        }

        currentGunTransform = player.transform.GetChild(curGun).gameObject;
        currentGunSprite = player.transform.GetChild(curGun).gameObject;
    }

    void Update()
    {
        // This is the gun switching / handling
        
        // Weapon Switch. CurGun statement is for inv size
        if ((Input.GetAxis("Mouse ScrollWheel") > 0) && (curGun > 0))
            curGun -= 1;
        if ((Input.GetAxis("Mouse ScrollWheel") < 0) && (curGun < gameObject.transform.childCount - 1))
            curGun += 1;

        // Weapon Switch Via Buttons (1.. 5)
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
                curGun = i;
        }
        

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (player.transform.childCount > 1)
                player.transform.GetChild(curGun).SetParent(null);

        }
        if (curGun == player.transform.childCount)
        {
            curGun--;
        }

        setActiveGun(curGun);
    }

    // Functions
    void setActiveGun(int index)
    {
        
        for (int i = 0; i < player.transform.childCount; i++)
        {
            if (i != index)
            {
                player.transform.GetChild(i).GetComponent<GunBehavior>().isShooting = false;
                player.transform.GetChild(i).gameObject.SetActive(false);

            }
            else
                player.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if((col.transform.tag == "Gun")&&(col.transform.parent == null))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if(player.transform.childCount >= INV_SIZE)
                {
                    player.transform.GetChild(curGun).SetParent(null);
                    col.transform.SetParent(gameObject.transform);
                    col.transform.SetSiblingIndex(curGun);
                }
                else
                {
                    col.transform.SetParent(gameObject.transform);
                }

            }
        }
    }
}
