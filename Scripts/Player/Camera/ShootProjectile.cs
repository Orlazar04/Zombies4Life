// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZombieSpace;

// This script is meant for the player shooting a projectile
// Dependencies: Level State, Weapon Manager, Settings Manager, Aiming Behavior
// Main Contributors: Olivia Lazar
public class ShootProjectile : MonoBehaviour
{
    [SerializeField]
    private float projectileSpeed = 15;

    [SerializeField]
    private AudioClip projectileSFX;

    private bool canShoot;              // Whether a projectile can be fired
    private WeaponManager weaponMan;

    // Start is called before the first frame update
    private void Start()
    {
        canShoot = true;
        weaponMan = FindObjectOfType<WeaponManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        // While the level is active
        if(LevelManager.IsLevelActive())
        {
            // If player has a ranged weapon
            if(WeaponManager.equippedWeapon == WeaponType.Ranged)
            {
                ManageShooting();
            }
            // Otherwise reset shooting
            else if (!canShoot)
            {
                canShoot = true;
            }
        }
    }

    // Manages the player's shooting
    private void ManageShooting()
    {
        // Shoot projectile on button click if the player can shoot
        if (Input.GetButton("Fire1") && canShoot && AimingBehavior.inRange && WeaponManager.ammo > 0)
        {
            Shoot();
        }
    }       

    // Initiates process for shooting a projectile
    private void Shoot()
    {
        WeaponManager.ammo--;
        weaponMan.UpdateAmmoGUI();

        Vector3 position = WeaponManager.launchPoint.position;
        Quaternion rotation = transform.rotation * Quaternion.Euler(90, 0, 0);

        GameObject projectile = Instantiate(WeaponManager.projectile, position, rotation);
        projectile.transform.SetParent(GameObject.FindGameObjectWithTag("Projectiles").transform);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);
        
        // Play sound
        if(SettingsManager.isSFXOn) AudioSource.PlayClipAtPoint(projectileSFX, transform.position);

        // Play ranged swing animation
        WeaponManager.rangedAnimator.SetTrigger("attack");

        StartCoroutine(CoolDownGun());
    }

    // Resets ability to shoot
    private IEnumerator CoolDownGun()
    {
        canShoot = false;
        yield return new WaitForSeconds(WeaponManager.fireRate);
        canShoot = true;
    }
}
