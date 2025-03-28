// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is for buildings getting hit
// Dependencies: Settings Manager
// Main Contributors: Grace Calianese
public class BuildingBehavior : MonoBehaviour
{
    private AudioSource hitBuildingSFX;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            if(SettingsManager.isSFXOn) hitBuildingSFX.Play();
        }
    }
}
