using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Camera mainCamera;
    public float radius;
    private GameObject player;

	void Start ()
    {
        player = GameObject.Find("Player");
	}

    void FixedUpdate()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -20;

        Vector2 newCamPos = Vector2.Lerp(player.transform.position, mousePos, 0.25f);

        mainCamera.transform.position = new Vector3(Mathf.Round(newCamPos.x * 25.0f)/ 25.0f , Mathf.Round(newCamPos.y * 25.0f)/25.0f, -25);
    }
}