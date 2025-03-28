// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieSpace;

// This script is meant for the player performing melee attacks
// Dependencies: Level State, Weapon Manager, Settings Manager, Aiming Behavior
// Main Contributors: Olivia Lazar, Tarif Khan
public class SwingMelee : MonoBehaviour
{
    [SerializeField]
    private AudioClip meleeSFX;

    public static bool canSwing;              // Whether the player can perform a melee attack

    // Start is called before the first frame update
    private void Start()
    {
        canSwing = true;
    }

    // Update is called once per frame
    private void Update()
    {
        // While the level is active
        if (LevelManager.IsLevelActive())
        {
            // If player has a melee weapon equipped
            if (WeaponManager.equippedWeapon == WeaponType.Melee)
            {
                ManageMeleeAttack();
            }
            // Otherwise reset swinging
            else if (!canSwing)
            {
                canSwing = true;
            }
        }
    }

    // Manages the player's melee attacks
    private void ManageMeleeAttack()
    {
        // Swing weapon on button click if the player can swing
        if (Input.GetButtonDown("Fire1") && canSwing && AimingBehavior.inRange)
        {
            Swing();
        }
    }

    // Initiates the melee attack
    private void Swing()
    {
        // Play melee swing animation
        WeaponManager.meleeAnimator.SetTrigger("attack");

        // Play swing sound effect
        if (SettingsManager.isSFXOn) AudioSource.PlayClipAtPoint(meleeSFX, transform.position);

        StartCoroutine(RechargeSwing());
    }

    // Resets ability to swing
    private IEnumerator RechargeSwing()
    {
        canSwing = false;
        yield return new WaitForSeconds(WeaponManager.swingRate);
        canSwing = true;
    }
}
