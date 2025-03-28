// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is for the exit of the level
// Main Contributors: Grace Calianese
public class ExitBehavior : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            FindObjectOfType<LevelManager>().LevelFinishAttempt();
        }
    }
}
