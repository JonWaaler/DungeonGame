using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class player_Health : MonoBehaviour {

    public PostProcessingProfile postProcessingProfile;
    public Slider player_HealthBar;
    private GameObject player;
    public GameObject deathParticles;
    public bool canDie;

    private void Start()
    {
        postProcessingProfile.vignette.enabled = false;
        postProcessingProfile.chromaticAberration.enabled = false;

        player_HealthBar = GameObject.Find("PlayerHealth").GetComponent<Slider>();
        player = GameObject.Find("Player");
    }

    void Update () {
        // Create New settings
        var settings = postProcessingProfile.vignette.settings;

        if (player_HealthBar.value < 20)
        {
            postProcessingProfile.vignette.enabled = true;

            // modify settings intensity
            settings.intensity = (1.0f/player_HealthBar.value)*4.0f;
            if (settings.intensity > 0.4f)
                settings.intensity = 0.4f;

            if(player_HealthBar.value <15)
                postProcessingProfile.chromaticAberration.enabled = true;
        }
        //else
            
            
        postProcessingProfile.vignette.settings = settings;

        if (player_HealthBar.value <= 0)
        {
            //play anim
            //play particles
            deathParticles.SetActive(true);
            deathParticles.transform.position = new Vector3(player.transform.position.x, player.transform.position.y,-1);
            deathParticles.GetComponent<ParticleSystem>().Play();

            // For debuging
            if(canDie)
                Destroy(player);
            else
                player_HealthBar.value = 100;
            
            
            postProcessingProfile.vignette.enabled = false;
            postProcessingProfile.chromaticAberration.enabled = false;
        }
    }

    // Bullet Collisions
    void OnTriggerEnter2D(Collider2D coll)
    {

        if ((coll.gameObject.tag == "Bullet_Boss"))
        {
            //Health Bar go down
            player_HealthBar.value -= 2.5f;
        }
        if(coll.gameObject.tag == "Boss")
            player_HealthBar.value -= 5f;

    }
}
