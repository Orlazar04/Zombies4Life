// Golden Version
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZombieSpace;

// This script is for a weapon's behavior
// Dependencies: Level State, Settings Manager
// Main Contributors: Olivia Lazar 
public class WeaponPickupBehavior : MonoBehaviour
{
    [SerializeField]
    private WeaponName weapon = WeaponName.None;        
    public WeaponScale damage = WeaponScale.Low;        // How much health an enemy loses
    public WeaponScale knockback = WeaponScale.Low;     // How far an enemy gets pushed back
    public WeaponScale rate = WeaponScale.Low;          // Amount of time before able to use weapon again
    public WeaponScale range = WeaponScale.Low;         // How far the weapon can reach

    [SerializeField]
    private GameObject statsCanvas;
    private GameObject panel;
    private Slider damageSldr;
    private Slider knockSldr;
    private Slider rateSldr;
    private Slider rangeSldr;

    [SerializeField]
    private float rotateSpeed = 90;

    [SerializeField]
    private AudioClip pickupSFX;
    [SerializeField]
    private GameObject pickupFX;

    private Transform playerTF;

    // Start is called before the first frame update
    private void Start()
    {
        playerTF = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    private void Update()
    {
        // While the level is not over and is a pickup
        if(LevelManager.IsLevelActive())
        {
            transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);

            if(panel != null)
            {
                FacePlayer();
            }
        }
    }

    // Activates the stats menu
    public void ActivateStatsMenu()
    {
        if(panel == null)
        {
            InitializeStatsGUI();
            StartCoroutine(DeactivateStatsMenu());
        }
    }

    // Initializes the values to hover over the weapon
    private void InitializeStatsGUI()
    {
        panel = Instantiate(statsCanvas, transform.position + statsCanvas.transform.position, statsCanvas.transform.rotation);
        panel.transform.SetParent(GameObject.FindGameObjectWithTag("Pickups").transform);

        Slider[] sliders = panel.GetComponentsInChildren<Slider>(true);
        damageSldr = Array.Find(sliders, slider => slider.name == "Damage Sldr");
        knockSldr = Array.Find(sliders, slider => slider.name == "Knock Sldr");
        rateSldr = Array.Find(sliders, slider => slider.name == "Rate Sldr");
        rangeSldr = Array.Find(sliders, slider => slider.name == "Range Sldr");

        damageSldr.value = (int) damage + 1;
        knockSldr.value = (int) knockback + 1;
        rateSldr.value = (int) rate + 1;
        rangeSldr.value = (int) range + 1;
    }

    // Rotates the GUI to face the player
    private void FacePlayer()
    {
        Vector3 directionToPlayer = transform.position - playerTF.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        Quaternion newRotation = Quaternion.Euler(lookRotation.eulerAngles.x, lookRotation.eulerAngles.y, 0);
        panel.transform.rotation = Quaternion.Slerp(panel.transform.rotation, newRotation, Time.deltaTime * 10);
    }

    // Closes the stats menu
    private IEnumerator DeactivateStatsMenu()
    {
        yield return new WaitForSeconds(3f);
        Destroy(panel);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(AttemptWeaponEquip())
            {
                if(SettingsManager.isSFXOn) AudioSource.PlayClipAtPoint(pickupSFX, transform.position);
                GameObject effect = Instantiate(pickupFX, transform.position, transform.rotation);
                effect.transform.SetParent(transform.parent);

                if(panel != null)
                {
                    Destroy(panel);
                }

                Destroy(gameObject);
            }
        }
    }

    // Equips the weapon if the weapon slot of the same type is empty
    // Returns whether the exchanged occured
    private bool AttemptWeaponEquip()
    {
        if(gameObject.CompareTag("Melee Weapon") && !WeaponManager.hasMelee)
        {
            FindObjectOfType<WeaponManager>().UpdateMeleeWeapon(weapon);
            return true;
        }
        else if(gameObject.CompareTag("Ranged Weapon") && !WeaponManager.hasRanged)
        {
            FindObjectOfType<WeaponManager>().UpdateRangedWeapon(weapon);
            return true;
        }

        return false;
    }
}