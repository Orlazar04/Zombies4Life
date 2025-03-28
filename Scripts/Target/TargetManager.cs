// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieSpace;

// Managing the status of the target pickup
// Dependencies: Level State, Target Zone Behavior
// Main Contributors: Olivia Lazar
public class TargetManager : MonoBehaviour
{
    public static TargetState state;    // The status of the target pickup
    public static string itemName;      // The name of the pickup
    public static float integrity;      // How much time until target pickup is destroyed
    public static int threateningZombies;
    public static int attackingZombies;

    private TargetPickupBehavior targetPickup;

    // Whether the target pickup is accessible (not collected or destroyed)
    public static bool IsTargetActive()
    {
        return (state != TargetState.Collected && state != TargetState.Destroyed);
    }

    // Awake is called before Start
    private void Awake()
    {
        state = TargetState.Safe;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize target pickup stats
        targetPickup = GameObject.FindGameObjectWithTag("Target Pickup").GetComponent<TargetPickupBehavior>();
        itemName = targetPickup.itemName;
        integrity = targetPickup.startIntegrity;

        threateningZombies = 0;
        attackingZombies = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        // While the level is not over, decrease target pickup integrity while being attacked
        if(LevelManager.IsLevelActive() && state == TargetState.Attacked)
        {
            // Decrease integrity timer by a factor relative to attacking zombie count
            if(integrity > 0)
            {
                float scaleFactor = 1 + (0.25f * (attackingZombies - 1));
                integrity -= (Time.deltaTime * scaleFactor);
            }
            // Destroy target pickup when integrity reaches zero
            else
            {
                state = TargetState.Destroyed;
                targetPickup.DestroyTarget();
            }
        }
    }

    // Updates the target pickup state based on the surrounding zombies
    public static void UpdateTargetState()
    {
        if(IsTargetActive())
        {
            if(attackingZombies > 0)
            {
                state = TargetState.Attacked;
            }
            else if(threateningZombies > 0)
            {
                state = TargetState.Threatened;
            }
            else
            {
                state = TargetState.Safe;
            }
        }
    }

    // Updates the status of the target pickup to collected if not already destroyed
    public static bool CollectTargetAttempt()
    {
        if(state != TargetState.Destroyed)
        {
            state = TargetState.Collected;
            return true;
        }
        return false;
    }

}
