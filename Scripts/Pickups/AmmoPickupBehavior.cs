// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script controls the effects of an ammo pickup
// Dependencies: Weapon Manager
// Main Contributors: Olivia Lazar
public class AmmoPickupBehavior : MonoBehaviour
{
    [SerializeField]
    private int bulletAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            WeaponManager.ammo += bulletAmount;
        }
    }
}
