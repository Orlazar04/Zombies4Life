// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZombieSpace;

// This script is meant for managing the player's weapons
// Dependencies: Level Manager, Level State, Save Manager
// Main Contributors: Olivia Lazar, Tarif Khan
public class WeaponManager : MonoBehaviour
{ 
    public static WeaponType equippedWeapon;

    // Melee Weapon Stats
    public static bool hasMelee;
    private static WeaponName meleeName;
    public static int meleeIndex;
    public static int meleeDamage;
    public static float meleeKnockback;
    public static float swingRate;
    public static float swingRange;

    public static GameObject meleeWeapon;
    public static Animator meleeAnimator;

    // Ranged Weapon Stats
    public static bool hasRanged;
    private static WeaponName rangedName;
    public static int rangedIndex;
    public static int projectileDamage;
    public static float projectileKnockback;
    public static float fireRate;
    public static float fireRange;
    public static int ammo = 0;

    public static GameObject rangedWeapon;
    public static GameObject projectile;
    public static Transform launchPoint;
    public static Animator rangedAnimator;

    [SerializeField]
    private GameObject[] meleeWeapons;
    [SerializeField]
    private GameObject[] rangedWeapons;
    [SerializeField]
    private GameObject[] bullets;
    [SerializeField]
    private int addAmmoAmount = 15;

    // GUI
    [SerializeField]
    private Text ammoTxt;
    [SerializeField]
    private Toggle meleeIcon;
    [SerializeField]
    private Toggle rangedIcon;
    [SerializeField]
    private Toggle dropIcon;

    private Transform cameraTF;

    // Start is called before the first frame update
    private void Start()
    {
        equippedWeapon = WeaponType.None;
        cameraTF = Camera.main.transform;

        InitializeWeapons();
        ammo += addAmmoAmount;

        // Initialize GUI
        UpdateAmmoGUI();
        UpdateWeaponsIcons();
    }

    // Loads previous weapons from past level or saved values
    private void InitializeWeapons()
    {
        // Reset values on first level
        if(LevelManager.IsFirstLevel())
        {
            UpdateMeleeWeapon(WeaponName.Crowbar);
            UpdateRangedWeapon(WeaponName.Pistol);
            ammo = 0;
        }
        // Load values if continued game
        else if(SaveManager.IsLoadedLevel())
        {
            int[] weapons = SaveManager.LoadWeapons();
            UpdateMeleeWeapon((WeaponName) weapons[0]);
            UpdateRangedWeapon((WeaponName) weapons[1]);
            ammo = weapons[2];
        }
        // Adds weapons from previous level
        else
        {
            UpdateMeleeWeapon(meleeName);
            UpdateRangedWeapon(rangedName);   
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // While the level is active
        if(LevelManager.IsLevelActive())
        {
            // Equiping weapons
            if(Input.GetKeyDown(KeyCode.E))
            {
                EquipWeaponType(WeaponType.Melee, hasMelee);
                UpdateWeaponsIcons();
            }
            else if(Input.GetKeyDown(KeyCode.R))
            {
                EquipWeaponType(WeaponType.Ranged, hasRanged);
                UpdateWeaponsIcons();
            }
            // Dropping weapons
            else if(Input.GetKeyDown(KeyCode.Q) && equippedWeapon != WeaponType.None)
            {
                DropEquippedWeapon();
                UpdateWeaponsIcons();
            }
        }
    }

    // Instantiates the weapon in the camera
    private GameObject AddToCamera(WeaponType type)
    {
        GameObject weapon;
        GameObject weaponPrefab = null;
        Vector3 position = Vector3.one;
        Quaternion rotation = Quaternion.Euler(Vector3.one);

        switch(type)
        {
            case WeaponType.Melee:
                weaponPrefab = meleeWeapons[meleeIndex];
                position = new Vector3(0.25f, -0.5f, 0.3f);
                rotation = Quaternion.Euler(new Vector3(0, 0, -15));
                break;
            case WeaponType.Ranged:
                weaponPrefab = rangedWeapons[rangedIndex];
                position = new Vector3(0.2f, -0.3f, 0.25f);
                rotation = Quaternion.Euler(new Vector3(-10, -10, 0));
                break;
        }

        Transform weaponTF = weaponPrefab.transform;

        weapon = Instantiate(weaponPrefab, cameraTF.position, cameraTF.rotation);
        DeactivatePickupMode(weapon);
        weapon.transform.parent = cameraTF;
        weapon.transform.localPosition = position;
        weapon.transform.localRotation = rotation;

        weapon.SetActive(false);

        return weapon;
    }

    // Deactivates components related to the weapon in pickup mode
    private void DeactivatePickupMode(GameObject weapon)
    {
        weapon.layer = LayerMask.NameToLayer("Ignore Raycast");
        weapon.GetComponent<WeaponPickupBehavior>().enabled = false;
        weapon.GetComponentInChildren<Light>().enabled = false;
    }

    // Changes the equipped weapon to the given type if possible
    private void EquipWeaponType(WeaponType type, bool hasWeapon)
    {
        // If given weapon type is not currently selected
        if(equippedWeapon != type)
        {
            // Switch to that weapon type if there is one
            if(hasWeapon)
            {
                if(equippedWeapon != WeaponType.None) SetWeaponActive(false);
                equippedWeapon = type;
                SetWeaponActive(true);
            }
            // Switch to no weapon if there is no weapon of that type
            else if(!hasWeapon && equippedWeapon != WeaponType.None)
            {
                SetWeaponActive(false);
                equippedWeapon = WeaponType.None;
            }
        }
    }
    
    // Places or removes the equipped weapon in the screen
    private void SetWeaponActive(bool isActive)
    {
        switch(equippedWeapon)
        {
            case WeaponType.Melee:
                meleeWeapon.SetActive(isActive);
                break;
            case WeaponType.Ranged:
                rangedWeapon.SetActive(isActive);
                break;
        }
    }

    // Drops the equipped weapon
    private void DropEquippedWeapon()
    {
        SetWeaponActive(false);
        switch(equippedWeapon)
        {
            case WeaponType.Melee:
                Destroy(meleeWeapon);
                SpawnWeaponPickup(meleeWeapons[meleeIndex]);
                UpdateMeleeWeapon(WeaponName.None);
                break;
            case WeaponType.Ranged:
                Destroy(rangedWeapon);
                SpawnWeaponPickup(rangedWeapons[rangedIndex]);
                UpdateRangedWeapon(WeaponName.None);
                break;
        }
    }

    // Places the given weapon into the world
    private void SpawnWeaponPickup(GameObject weapon)
    {
        GameObject weaponPickup = Instantiate(weapon, cameraTF.position + (2 * cameraTF.forward), weapon.transform.rotation);
        Transform weaponTF = weaponPickup.transform;
        weaponTF.position = new Vector3(weaponTF.position.x, 1.5f, weaponTF.position.z);
        weaponTF.parent = GameObject.FindGameObjectWithTag("Pickups").transform;
    }

    // Updates the behavior of the current melee weapon
    public void UpdateMeleeWeapon(WeaponName weapon) 
    {
        hasMelee = true;
        switch(weapon)
        {
            case WeaponName.None:
                hasMelee = false;
                equippedWeapon = WeaponType.None;
                meleeName = WeaponName.None;
                meleeIndex = -1;
                break;
            case WeaponName.Crowbar:
                meleeName = WeaponName.Crowbar;
                meleeIndex = 0;
                SetMeleeWeapon();
                break;
            case WeaponName.Bat:
                meleeName = WeaponName.Bat;
                meleeIndex = 1;
                SetMeleeWeapon();
                break;
            case WeaponName.Sledge:
                meleeName = WeaponName.Sledge;
                meleeIndex = 2;
                SetMeleeWeapon();
                break;
            case WeaponName.Axe:
                meleeName = WeaponName.Axe;
                meleeIndex = 3;
                SetMeleeWeapon();
                break;
        }
        UpdateWeaponsIcons();
    }

    // Sets the values of the current melee weapon
    private void SetMeleeWeapon()
    {
        meleeWeapon = AddToCamera(WeaponType.Melee);

        WeaponPickupBehavior weaponBehavior = meleeWeapon.GetComponent<WeaponPickupBehavior>();
        meleeDamage = (int) GetRelativeValue(weaponBehavior.damage, 10, 20, 30);
        meleeKnockback = GetRelativeValue(weaponBehavior.knockback, 2, 5, 7);
        swingRate = GetRelativeValue(weaponBehavior.rate, 2, 1, 0.5f);
        swingRange = GetRelativeValue(weaponBehavior.range, 1.5f, 2, 2.5f);

        meleeWeapon.GetComponent<Collider>().isTrigger = false;

        meleeAnimator = meleeWeapon.GetComponent<Animator>();
        meleeAnimator.enabled = true;
    }

    // Updates the behavior of the current ranged weapon
    public void UpdateRangedWeapon(WeaponName weapon) 
    {
        hasRanged = true;
        switch(weapon)
        {
            case WeaponName.None:
                hasRanged = false;
                equippedWeapon = WeaponType.None;
                rangedName = WeaponName.None;
                rangedIndex = -1;
                break;
            case WeaponName.Pistol:
                rangedName = WeaponName.Pistol;
                rangedIndex = 0;
                SetRangedWeapon();
                break;
            case WeaponName.SMG:
                rangedName = WeaponName.SMG;
                rangedIndex = 1;
                SetRangedWeapon();
                break;
            case WeaponName.Machine:
                rangedName = WeaponName.Machine;
                rangedIndex = 2;
                SetRangedWeapon();
                break;
            case WeaponName.Shotgun:
                rangedName = WeaponName.Shotgun;
                rangedIndex = 3;
                SetRangedWeapon();
                break;
            case WeaponName.Sniper:
                rangedName = WeaponName.None;
                rangedIndex = 4;
                SetRangedWeapon();
                break;
        }
        UpdateWeaponsIcons();
    }

    // Sets the values of the current ranged weapon
    private void SetRangedWeapon()
    {
        rangedWeapon = AddToCamera(WeaponType.Ranged);

        WeaponPickupBehavior weaponBehavior = rangedWeapon.GetComponent<WeaponPickupBehavior>();
        projectileDamage = (int) GetRelativeValue(weaponBehavior.damage, 10, 20, 30);
        projectileKnockback = GetRelativeValue(weaponBehavior.knockback, 10, 15, 20);
        fireRate = GetRelativeValue(weaponBehavior.rate, 2, 1, 0.5f);
        fireRange = GetRelativeValue(weaponBehavior.range, 7, 12, 20);

        projectile = bullets[rangedIndex];
        launchPoint = rangedWeapon.transform.GetChild(0);
        rangedAnimator = rangedWeapon.GetComponent<Animator>();
        rangedAnimator.enabled = true;
    }

    // Returns the relative value based on the given scale
    private float GetRelativeValue(WeaponScale scale, float low, float mid, float high)
    {
        float relative = low;
        switch(scale)
        {
            case WeaponScale.Low:
                relative = low;
                break;
            case WeaponScale.Mid:
                relative = mid;
                break;
            case WeaponScale.High:
                relative = high;
                break;
        }
        return relative;
    }

    // Updates the value of the ammo counter GUI
    public void UpdateAmmoGUI()
    {
        ammoTxt.text = ammo.ToString();
    }

    // Updates the visuals of whether a weapon type is available to equip
    private void UpdateWeaponsIcons()
    {
        meleeIcon.isOn = hasMelee;
        rangedIcon.isOn = hasRanged;
        dropIcon.isOn = (equippedWeapon != WeaponType.None);
    }
}