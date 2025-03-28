// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script controls the effects of an armor pickup
// Main Contributors: Trin Rist
public class ArmorPickupBehavior : MonoBehaviour
{
    [SerializeField]
    private int armorStrength = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            playerHealth.UpdateProtection(armorStrength);

            Destroy(gameObject);
        }
    }
}