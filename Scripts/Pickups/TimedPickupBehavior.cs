// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script controls behavior for pickups that despawn
// Dependencies: Level State, Settings Manager
// Main Contributors: Grace Calianese, Olivia Lazar
public class TimedPickupBehavior : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed = 90;
    [SerializeField]
    private float bobHeight = 2;
    [SerializeField]
    private float bobSpeed = 1;

    [SerializeField]
    private AudioClip pickupSFX;
    [SerializeField]
    private GameObject pickupFX;

    private Vector3 startPosition;

    // Start is called before the first frame update
    private void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        // While the level is not over
        if(LevelManager.IsLevelActive())
        {
            HoverAnimation();
        }
    }

    // Updates the hovering animation
    private void HoverAnimation()
    {
        transform.Rotate(transform.up, Time.deltaTime * rotateSpeed);

        float newY = startPosition.y + (bobHeight * Mathf.Sin(Time.time * bobSpeed));
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(SettingsManager.isSFXOn) AudioSource.PlayClipAtPoint(pickupSFX, transform.position);
            GameObject effect = Instantiate(pickupFX, transform.position, pickupFX.transform.rotation);
            effect.transform.SetParent(transform.parent);

            Destroy(gameObject);
        }
    }
}
