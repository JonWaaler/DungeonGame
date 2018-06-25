using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPerfectCam : MonoBehaviour {

    public Camera cam;

	void Update () {

        cam.orthographicSize = Screen.height / 2;
    }
}
