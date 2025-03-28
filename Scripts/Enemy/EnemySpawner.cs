// Golden Version
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script controls spawning of enemies
// Dependencies: Level State
// Main Contributors: Olivia Lazar, Grace Calianese
public class EnemySpawner : MonoBehaviour
{
    public static int enemyCount;

    [SerializeField]
    private float spawnStartTime = 3;
    [SerializeField]
    private float spawnPauseTime = 3;
    [SerializeField]
    private float gracePeriod = 15f;            // Amount of time before spawn near target pickup

    [SerializeField]
    private GameObject[] enemyPrefabs;

    private Transform playerTF;
    private Transform targetTF;

    private float startTime;
    private Transform[] spawnPoints;            // All places where enemies can spawn
    private int difficulty;
    private Transform[] activeSpawnPoints;      // Where enemies can spawn currently
    private Transform[] pickupSpawnPoints;      // Spawn points closest to target pickup

    // Start is called before the first frame update
    private void Start()
    {
        startTime = Time.time;

        difficulty = LevelManager.levelDifficulty;
        playerTF = GameObject.FindGameObjectWithTag("Player").transform;

        // Reset enemy count to the amount starting out in the level
        enemyCount = GameObject.FindGameObjectsWithTag("Zombie").Length;

        // Initialize spawn point positions
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point").Select(go => go.transform).ToArray();
        pickupSpawnPoints = GameObject.FindGameObjectsWithTag("Pickup Spawn Points").Select(go => go.transform).ToArray();

        InvokeRepeating("SpawnEnemies", spawnStartTime, spawnPauseTime);
    }

    private void Update()
    {
        Debug.Log("enemyCount: " + enemyCount + " max: " + (5 * difficulty));
    }

    // Possibly spawn random enemy at random location near the player or target pickup
    private void SpawnEnemies()
    {
        // While the level is not over
        if(LevelManager.IsLevelActive())
        {
            var spawnChance = Random.Range(0, 150 / difficulty);

            if(spawnChance < 50 && enemyCount < (5 * difficulty))
            {
                // Increase enemy count
                enemyCount++;
                UpdateSpawnPoints();
                int index = Random.Range(0, activeSpawnPoints.Length);
                Transform enemyPosition = activeSpawnPoints[index];

                // Spawn enemy
                GameObject spawnedEnemy = Instantiate(RandomEnemy(), enemyPosition.position, enemyPosition.rotation);
                spawnedEnemy.transform.parent = gameObject.transform.parent; 
            }
        }
    }

    // Update which spawn points are active
    private void UpdateSpawnPoints()
    {
        Transform[] updatedPoints = spawnPoints.OrderBy(point => Vector3.Distance(playerTF.position, point.position))
                  .Take(3)
                  .ToArray();

        // If enough time passed, also spawn enemies at the two closet points to target pickup
        if((Time.time - startTime) >= gracePeriod)
        {
            updatedPoints = updatedPoints.Concat(pickupSpawnPoints).Distinct().ToArray();
        }
        activeSpawnPoints = updatedPoints;
    }

    // Select a random enemy to spawn
    private GameObject RandomEnemy()
    {
        int enemyIndex = 0;
        int spawnType = Random.Range(0, 100);

        if(spawnType < 20)
        {
            enemyIndex = 2;
        }
        else if(spawnType < 50)
        {
            enemyIndex = 1;
        }

        return enemyPrefabs[enemyIndex];
    }    
}
