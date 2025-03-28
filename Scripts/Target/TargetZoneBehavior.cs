// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieSpace;

// This script is for the zones near the target pick up detecting zombies
// Dependencies: Target Manager
// Main Contributors: Olivia Lazar, Grace Calianese
public class TargetZoneBehavior : MonoBehaviour
{
    [SerializeField]
    private TargetState zoneType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            UpdateZombieCount(1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            UpdateZombieCount(-1);
        }
    }

    // Updates the count of zombies in the proper zone
    private void UpdateZombieCount(int zombie)
    {
        switch(zoneType)
        {
            case TargetState.Threatened:
                TargetManager.threateningZombies += zombie;
                break;
            case TargetState.Attacked:
                TargetManager.attackingZombies += zombie;
                break;
        }
        TargetManager.UpdateTargetState();
    }
}
