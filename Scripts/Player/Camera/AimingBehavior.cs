// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZombieSpace;

// This script is for the components affected by what is looked at
// Dependencies: Level State, Weapon Manager
// Main Contributors: Olivia Lazar
public class AimingBehavior : MonoBehaviour
{
    public static bool inRange = false;

    [SerializeField]
    private Image[] reticles;               // Reticle image (0: Melee, 1: Ranged)
    [SerializeField]
    private Color[] reticleColors;          // Reticle color options (0: Base, 1: Highlight)

    private WeaponType currentWeapon;       // Current weapon type equipped
    private Image currentReticle;           // The current reticle selected
    private float currentRange;             // The distance of the ray cast

    // Start is called before the first frame update
    private void Start()
    {
        UpdateReticleMode();
    }

    // Update is called once per frame
    private void Update()
    {
        // While the level is active
        if(LevelManager.IsLevelActive())
        {
            // When the current selected weapon has switched
            if(currentWeapon != WeaponManager.equippedWeapon)
            {
                UpdateReticleMode();
            }
        }
    }

    // Updates how the reticle behaves
    private void UpdateReticleMode()
    {
        currentWeapon = WeaponManager.equippedWeapon;
        reticles[0].gameObject.SetActive(false);
        reticles[1].gameObject.SetActive(false);
        currentReticle = reticles[0];

        switch(currentWeapon)
        {
            case WeaponType.None:
                currentRange = 0;
                break;
            case WeaponType.Melee:
                currentReticle = reticles[0];
                currentReticle.gameObject.SetActive(true);
                currentRange = WeaponManager.swingRange;
                break;
            case WeaponType.Ranged:
                currentReticle = reticles[1];
                currentReticle.gameObject.SetActive(true);
                currentRange = WeaponManager.fireRange;
                break;
        }
    }

    // Update for physics based components
    private void FixedUpdate()
    {
        RaycastHit hit;
        // Update reticle based on enemy in scope
        if(Physics.Raycast(transform.position, transform.forward, out hit, currentRange))
        {
            if(hit.collider.CompareTag("Zombie") || hit.collider.CompareTag("Breakable"))
            {
                UpdateAim(true, reticleColors[1]);
            }
            else
            {
                UpdateAim(false, reticleColors[0]);
            }
        }
        else
        {
            UpdateAim(false, reticleColors[0]);
        }

        // Activate weapon stats menu if weapon pickup in scope
        if(Physics.Raycast(transform.position, transform.forward, out hit, 3))
        {
            if(hit.collider.CompareTag("Melee Weapon") || hit.collider.CompareTag("Ranged Weapon"))
            {
                hit.collider.GetComponent<WeaponPickupBehavior>().ActivateStatsMenu();
            }
        }
    }

    // Updates values based on what is being looked at
    private void UpdateAim(bool range, Color color)
    {
        inRange = range;
        UpdateReticle(color);
    }

    // Updates the look of the reticle
    private void UpdateReticle(Color color)
    {
        // Scale
        Vector3 scale = Vector3.one;
        if(color == reticleColors[1])
        {
            scale = new Vector3(0.75f, 0.75f, 1);
        }

        currentReticle.color = Color.Lerp(currentReticle.color, color, Time.fixedDeltaTime * 5);
        currentReticle.transform.localScale = Vector3.Lerp(currentReticle.transform.localScale, scale, Time.fixedDeltaTime * 3);
    }
}
