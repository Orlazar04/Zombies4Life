// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZombieSpace;

// This script is meant for the player's health management
// Dependencies: Level Difficulty, Level State, Save Manager, Settings Manager
// Main Contributors: Olivia Lazar, Trin Rist
public class PlayerHealth : MonoBehaviour
{
    public static int protection = 0;   // The current amount of protection afforded by the player's armor
    public static int maxProtection;    // The maximum protection a player can have

    [SerializeField]
    private int startHealth = 200;      // The maximum amount of starting health
    [SerializeField]
    private int reduceHealth = 25;      // The factor by which maximum health is reduced for a level

    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private Slider armorSlider;
    [SerializeField]
    private AudioClip deadSFX;

    private int maxHealth;              // The maximum amount of health the player can have
    private int currentHealth;          // The current amount of health the player has
    
    private AudioSource hitSFX;

    // Start is called before the first frame update
    private void Start()
    {
        // Initiate health and GUI
        maxHealth = startHealth - (reduceHealth * (LevelManager.levelDifficulty - 1));
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        // Initiate armor and GUI
        maxProtection = LevelManager.levelDifficulty * 10;
        armorSlider.maxValue = maxProtection;
        InitializeArmor();
        armorSlider.value = protection;

        // Initialize sound
        hitSFX = gameObject.GetComponent<AudioSource>();
    }

    // Loads previous armor from saved values
    private void InitializeArmor()
    {
        // Reset values on first level
        if(LevelManager.IsFirstLevel())
        {
            protection = 0;
        }
        // Load values if continued game
        else if(SaveManager.IsLoadedLevel())
        {
            protection = SaveManager.LoadArmor();
        }
    }

    // Decrease current health
    public void TakeDamage(int damageAmount)
    {
        // While the level is not over
        if (LevelManager.IsLevelActive())
        {
            if (currentHealth > 0)
            {
                int percentage = Mathf.RoundToInt((100 - protection) / 100);
                currentHealth -= (damageAmount * percentage);
                currentHealth = Mathf.Max(currentHealth, 0);
                healthSlider.value = currentHealth;
                
                if(SettingsManager.isSFXOn) hitSFX.Play();

                if (currentHealth == 0)
                {
                    PlayerDies();
                }
            }
        }
    }

    // Increase current health
    public void TakeHealth(int healthAmount)
    {
        if(currentHealth < maxHealth)
        {
            currentHealth += healthAmount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            healthSlider.value = currentHealth;
        }
    }

    // Initiates procedure for when player dies
    private void PlayerDies()
    {
        if(SettingsManager.isSFXOn) AudioSource.PlayClipAtPoint(deadSFX, transform.position);
        transform.Rotate(-90, 0, 0, Space.Self);

        FindObjectOfType<LevelManager>().LevelLost(DefeatType.PlayerKilled);
    }

    // Updates armor protection value and GUI
    public void UpdateProtection(int amount)
    {
        protection += amount;
        protection = Mathf.Min(protection, maxProtection);
        armorSlider.value = protection;
    }
}
