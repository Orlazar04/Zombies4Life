// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script controls the effects of a health pickup
// Main Contributors: Olivia Lazar
public class HealthPickupBehavior : MonoBehaviour
{
    [SerializeField]
    private int healthAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth.TakeHealth(healthAmount);
        }
    }
}
