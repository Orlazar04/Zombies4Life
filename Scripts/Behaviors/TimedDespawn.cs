// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is meant for objects that despawn after an amount of time
// Dependencies: Level State
// Main Contributors: Olivia Lazar
public class TimedDespawn : MonoBehaviour
{
    [SerializeField]
    private float duration = 2;     // The amount of time before the object despawns

    private float timer = 0;        // The current amount of time of the object's existence

    // Update is called once per frame
    private void Update()
    {
        // While the level is not over
        if(LevelManager.IsLevelActive())
        {   
            // Destroy object after some time
            timer += Time.deltaTime;
            if(timer >= duration)
            {
                Destroy(gameObject);
            }
        }
    }
}
