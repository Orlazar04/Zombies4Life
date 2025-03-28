// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieSpace;
using UnityEngine.UI;

// This script is for enemy behavior
// Dependencies: Level State, Level Difficulty, Settings Manager, Swing Melee, Weapon Manager
// Main Contributors: Grace Calianese, Olivia Lazar 
public class EnemyBehavior : MonoBehaviour
{
    public static int enemyType;

    [SerializeField, Range(1,3)]
    private int setEnemyType;
    [SerializeField]
    private int health;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float displayDuration = 3f;

    private bool isNewStrike;               // Refering to zombie attack
    private GameObject player;
    private PlayerHealth playerHealth;
    private EnemyAI enemyAI;
    private bool isNewSwing;                // Refering to melee attack

    private Rigidbody rb;
    private Slider healthSlider;
    private AudioSource zombieHitSFX;
    private Animator anim;

    // Awake is called before Start
    private void Awake()
    {
        enemyType = setEnemyType;
    }

    // Start is called before the first frame update
    private void Start()
    {
        InitializeEnemyType();
        enemyAI = GetComponent<EnemyAI>();

        // Initialize sync with animation
        anim = GetComponent<Animator>();
        isNewStrike = true;
        rb = GetComponent<Rigidbody>();
        zombieHitSFX = GetComponent<AudioSource>();
        isNewSwing = true;

        // Initialize health GUI
        healthSlider = GetComponentInChildren<Slider>(true);
        healthSlider.gameObject.SetActive(false);
        healthSlider.value = health;
        healthSlider.maxValue = health;

        // Initialize player components
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    // Initialize enemy stats, affected by difficulty and enemy type
    private void InitializeEnemyType()
    {
        switch(enemyType)
        {
            case 1:
                SetEnemyValues(30, 15);
                break;
            case 2:
                SetEnemyValues(50, 20);
                break;
            case 3:
                SetEnemyValues(70, 30);
                break;
        }
    }

    // Sets the enemy values based on difficulty
    private void SetEnemyValues(int healthBase, int damageBase)
    {
        health = healthBase + (20 * (LevelManager.levelDifficulty - 1));
        damage = damageBase + (5 * (LevelManager.levelDifficulty - 1));
    }

    // Update is called once per frame
    private void Update()
    {
        // While the level is active
        if (LevelManager.IsLevelActive())
        {
            UpdateHealthGUI();
        }
    }

    // Updates the health GUI to face the player
    private void UpdateHealthGUI()
    {
        if(healthSlider.IsActive())
        {
            Vector3 directionToPlayer = transform.position - player.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            Quaternion newRotation = Quaternion.Euler(lookRotation.eulerAngles.x, lookRotation.eulerAngles.y, 0);
            healthSlider.transform.rotation = Quaternion.Slerp(healthSlider.transform.rotation, newRotation, Time.deltaTime * 10);
        }
    }
        
    private void OnTriggerStay(Collider other)
    {
        // Initiates procedure for attacking player
        if (other.gameObject.CompareTag("Player") && enemyAI.IsAlive())
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            float progression = stateInfo.normalizedTime % 1;
            bool isContact = stateInfo.IsName("Attack") && progression >= 0.44f && progression <= 0.46f;

            // For a new strike
            if(isNewStrike && isContact)
            {
                playerHealth.TakeDamage(damage);
                isNewStrike = false;
            }
            // For an occuring strike
            else if (progression < 0.02f)
            {
                isNewStrike = true;
            }
        }

        // Receiving damage from melee weapons
        if (other.gameObject.CompareTag("Melee Weapon") && !SwingMelee.canSwing && isNewSwing)
        {   
            StartCoroutine(ReceiveSwing(other.gameObject));
        }
    }

    // Initiates procedure for when enemy is hit with a melee weapon
    private IEnumerator ReceiveSwing(GameObject weapon)
    {
        isNewSwing = false;

        // Wait for swing to peak
        yield return new WaitForSeconds(0.2f);
        TakeDamage(WeaponManager.meleeDamage);
        KnockbackEnemy(weapon, WeaponManager.meleeKnockback);

        // Wait for swing to finish
        yield return new WaitForSeconds(WeaponManager.swingRate);
        isNewSwing = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Receiving damage from ranged weapons
        if (other.gameObject.CompareTag("Projectile"))
        {
            TakeDamage(WeaponManager.projectileDamage);
            KnockbackEnemy(other.gameObject, WeaponManager.projectileKnockback);
        }
    }

    // Decreases the health of the enemy by the given amount
    private void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        health = Mathf.Max(health, 0);
        healthSlider.value = health;

        if(SettingsManager.isSFXOn) zombieHitSFX.Play();

        // Kill zombie with zero health
        if (health <= 0 &&  enemyAI.currentState != ZombieState.Dead)
        {
            enemyAI.ZombieDies();
        }

        StartCoroutine(DisplayHealthBar());
    }

    // Displays the enemy health bar for a few seconds
    private IEnumerator DisplayHealthBar()
    {
        healthSlider.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        healthSlider.gameObject.SetActive(false);
    }

    // Add knockback to the enemy getting hit
    private void KnockbackEnemy(GameObject forceObject, float forceAmount)
    {
        rb.isKinematic = false;

        Vector3 forceDirection = transform.position - forceObject.transform.position;
        forceDirection.y = 0;
        forceDirection.Normalize();

        rb.AddForce(forceDirection * forceAmount, ForceMode.Impulse);
        StartCoroutine(ResetKinematics());
    }

    private IEnumerator ResetKinematics()
    {
        yield return new WaitForSeconds(0.5f);
        rb.isKinematic = true;
    }
}