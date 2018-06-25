using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

    public List<GameObject> Items;
    public int chestHP = 3;
    public float lerpTime = 0;
    public bool startLerp = false;
    public bool hasSpawned = false;
    int x;
    int y;
    GameObject instance;

    // Use this for initialization
    void Start () {
        x = Random.Range(-5, 5);
        y = Random.Range(-5, 5);

    }
	
	// Update is called once per frame
	void Update () {
        if (startLerp)
        {
            if (!hasSpawned)
            {
                hasSpawned = true;
                instance = Instantiate(Items[Random.Range(0, Items.Count - 1)]);
            }
            lerpTime += Time.deltaTime/2.0f;
            instance.transform.position = (Vector2.Lerp(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), new Vector2(gameObject.transform.position.x + x, gameObject.transform.position.y + y), lerpTime));
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.gameObject.tag == "Bullet_Player_Regular")
        {
            chestHP--;

            if (chestHP <= 0)
            {
                //GameObject instance = Instantiate(Items[Random.Range(0, Items.Count - 1)]);
                instance = Instantiate(Items[Random.Range(0, Items.Count - 1)]);
                startLerp = true;

            }
        }
    }
}
