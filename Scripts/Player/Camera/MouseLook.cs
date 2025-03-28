// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script serves is for the player's ability to look around.
// Dependencies: Settings Manager, Level State
// Main Contributors: Olivia Lazar
public class MouseLook : MonoBehaviour
{
    private Transform playerTF;
    private float pitch;

    // Start is called before the first frame update
    private void Start()
    {
        playerTF = transform.parent.transform;
    }

    // Update is called once per frame
    private void Update()
    {
        // While the level is active
        if(LevelManager.IsLevelActive())
        {
            float moveX = Input.GetAxis("Mouse X") * SettingsManager.mouseSensitivity * Time.deltaTime;
            float moveY = Input.GetAxis("Mouse Y") * SettingsManager.mouseSensitivity * Time.deltaTime;  

            playerTF.Rotate(Vector3.up * moveX);

            pitch -= moveY;
            pitch = Mathf.Clamp(pitch, -90f, 75f);
            transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        }
    }
}
