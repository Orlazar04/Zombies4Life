// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is for breakable objects
// Dependencies: Weapon Manager, Settings Manager, Player Health
// Main Contributors: Grace Calianese
public class BreakableBehavior : MonoBehaviour
{
    [SerializeField]
    private float explosionForce = 20f;
    [SerializeField]
    private float explosionRadius = 8f;

    [SerializeField]
    private GameObject cratePieces;
    [SerializeField]
    private GameObject[] pickups;
    [SerializeField]
    private AudioClip breakableSFX;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            DestroyCrate();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Melee Weapon") && !SwingMelee.canSwing)
        {
            DestroyCrate();
        }
    }

    // Explodes the crate
    private void DestroyCrate()
    {
        GameObject pieces = Instantiate(cratePieces, transform.position, transform.rotation);
        Rigidbody[] rbPieces = pieces.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbPieces)
        {
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        if(SettingsManager.isSFXOn) AudioSource.PlayClipAtPoint(breakableSFX, transform.position);
        
        DropAmmoPickup();

        Destroy(gameObject);
    }

    // Possibly instantiates an ammo or armor pickup
    private void DropAmmoPickup()
    {
        int maxIndex = pickups.Length;
        int spawnChance = Random.Range(0, 100 + (LevelManager.levelDifficulty * 50));

        // Increase chance for ammo spawn if low on ammo
        if(WeaponManager.ammo == 0)
        {
            spawnChance = Random.Range(0, 100);
            maxIndex -= 2;
        }

        if(spawnChance < 50)
        {
            // Prevent armor spawing if max protection already reached
            if(PlayerHealth.protection == PlayerHealth.maxProtection)
            {
                maxIndex -= 2;
            }

            int spawnType = Random.Range(0, maxIndex);

            Vector3 pickupPos = new Vector3(transform.position.x, 1.5f, transform.position.z);

            GameObject ammoPickup = Instantiate(pickups[spawnType], pickupPos, transform.rotation);
            ammoPickup.transform.parent = GameObject.FindGameObjectWithTag("Pickups").transform;
        }
    }
}
