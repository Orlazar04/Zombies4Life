// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZombieSpace;

// This script controls the target pickup behavior
// Dependencies: Level State, Target Manager, Settings Manager
// Main Contributors: Olivia Lazar
public class TargetPickupBehavior : MonoBehaviour
{
    public string itemName;
    public float startIntegrity;
    [SerializeField]
    private float requiredTime = 5f;
    [SerializeField]
    private float rotateSpeed = 45f;

    [SerializeField]
    private AudioClip pickupSFX;
    [SerializeField]
    private GameObject pickupFX;
    [SerializeField]
    private AudioClip destroyedSFX;
    [SerializeField]
    private GameObject destroyedFX;

    private bool playerIsNearby;
    private float timer = 0f;
    private Canvas panel;
    private Slider timerSldr;
    private Transform playerTF;

    // Start is called before the first frame update
    private void Start()
    {
        panel = GetComponentInChildren<Canvas>(true);
        timerSldr = GetComponentInChildren<Slider>(true);
        timerSldr.maxValue = requiredTime;

        playerTF = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    private void Update()
    {
        // While the level is not over
        if (LevelManager.IsLevelActive())
        {
            transform.Rotate(transform.up, Time.deltaTime * rotateSpeed);

            UpdateCollectionTimer();
            UpdateTimerGUI();
        }
    }

    // Updates the value of the collection timer based on the player's position
    private void UpdateCollectionTimer()
    {
        // Increase the timer if player is nearby
        if (playerIsNearby)
        {
            timer += Time.deltaTime;

            // Check if the required time has passed
            if (timer >= requiredTime)
            {
                // Collect target if not destroyed
                if(TargetManager.CollectTargetAttempt())
                {
                    CollectTarget();
                }
            }
        }
        // Deccrease the timer if player leaves
        else if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    // Updates the timer GUI
    private void UpdateTimerGUI()
    {
        if(timer > 0)
        {   
            // Activate Slider
            if(!panel.enabled)
            {
                panel.enabled = true;
            }

            timerSldr.value = timer;
            FacePlayer();
        }
        // Deactivate Slider
        else if (panel.enabled)
        {
            panel.enabled = false;
        }
    }

    // Rotates the GUI to face the player
    private void FacePlayer()
    {
        if(timerSldr.enabled)
        {
            Vector3 directionToPlayer = transform.position - playerTF.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            timerSldr.transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;
        }
    }

    // Initiates procedure for when target pickup is collected
    private void CollectTarget()
    {
        if(SettingsManager.isSFXOn) AudioSource.PlayClipAtPoint(pickupSFX, transform.position);
        GameObject effect = Instantiate(pickupFX, transform.position, pickupFX.transform.rotation);
        effect.transform.SetParent(transform.parent);

        Destroy(gameObject);
    }

    // Initiates procedure for destroying target pickup
    public void DestroyTarget()
    {
        if(SettingsManager.isSFXOn) AudioSource.PlayClipAtPoint(destroyedSFX, transform.position);
        GameObject effect = Instantiate(destroyedFX, transform.position, destroyedFX.transform.rotation);
        effect.transform.SetParent(transform.parent);

        FindObjectOfType<LevelManager>().LevelLost(DefeatType.TargetDestroyed);

        Destroy(gameObject);
    }
}
