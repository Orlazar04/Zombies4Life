// Golden Version
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Managing the setitngs of the game
// Dependencies: Save Manager, Confirmation Manager
// Main Contributors: Olivia Lazar
public class SettingsManager : MonoBehaviour
{
    public static bool isMusicOn = true;
    public static bool isSFXOn = true;
    public static int mouseSensitivity = 500;

    [SerializeField]
    private GameObject settingsMenu;

    private GameObject window;
    private Button exitBtn;
    private Button deleteBtn;
    private Toggle musicTgl;
    private Image[] musicIcons;
    private Toggle SFXTgl;
    private Image[] SFXIcons;
    private Slider mouseSensitivitySldr;

    // Start is called before the first frame update
    private void Start()
    {
        UpdateMusic();
    }

    // Opens the settings menu
    public void OpenSettingsMenu()
    {
        window = Instantiate(settingsMenu);
        InitializeGUI();
        UpdateGUI();
    }

    // Initialize connections to the menu GUI components
    private void InitializeGUI()
    {
        Button[] buttons = window.GetComponentsInChildren<Button>(true);
        exitBtn = Array.Find(buttons, button => button.name == "Exit Btn");
        deleteBtn = Array.Find(buttons, button => button.name == "Delete Btn");

        exitBtn.onClick.AddListener(CloseSettingsMenu);
        deleteBtn.onClick.AddListener(DeleteDataAttempt);

        Toggle[] toggles = window.GetComponentsInChildren<Toggle>(true);
        musicTgl = Array.Find(toggles, toggle => toggle.name == "Music Tgl");
        SFXTgl = Array.Find(toggles, toggle => toggle.name == "SFX Tgl");

        musicTgl.onValueChanged.AddListener(ToggleMusic);
        SFXTgl.onValueChanged.AddListener(ToggleSFX);

        Image[] images = window.GetComponentsInChildren<Image>(true);
        musicIcons = new Image[2];
        musicIcons[0] = Array.Find(images, image => image.name == "Music Icon Off");
        musicIcons[1] = Array.Find(images, image => image.name == "Music Icon On");
        SFXIcons = new Image[2];
        SFXIcons[0] = Array.Find(images, image => image.name == "SFX Icon Off");
        SFXIcons[1] = Array.Find(images, image => image.name == "SFX Icon On");

        mouseSensitivitySldr = window.GetComponentInChildren<Slider>(true);
        mouseSensitivitySldr.onValueChanged.AddListener(UpdateMouseSensesitivty);
    }

    // Updates the GUI to match the their respective settings value
    private void UpdateGUI()
    {
        musicTgl.isOn = isMusicOn;
        UpdateIcons(musicIcons, isMusicOn);
        SFXTgl.isOn = isSFXOn;
        UpdateIcons(SFXIcons, isSFXOn);
        mouseSensitivitySldr.value = (mouseSensitivity / 50);
    }

    // Closes the settings menu
    private void CloseSettingsMenu()
    {
        Destroy(window);
    }

    // Switches the state of the music
    private void ToggleMusic(bool isOn)
    {
        isMusicOn = isOn;
        UpdateIcons(musicIcons, isMusicOn);
        UpdateMusic();
    }

    // Updates the background music
    private void UpdateMusic()
    {
        Camera.main.GetComponent<AudioSource>().mute = !isMusicOn;
    }

    // Switches the state of the SFX
    private void ToggleSFX(bool isOn)
    {
        isSFXOn = isOn;
        UpdateIcons(SFXIcons, isSFXOn);
    }

    // Updates the image icons to match the toggles
    private void UpdateIcons(Image[] icons, bool isOn)
    {
        icons[0].gameObject.SetActive(false);
        icons[1].gameObject.SetActive(false);
        if(isOn)
        {
            icons[1].gameObject.SetActive(true);
        }
        else
        {
            icons[0].gameObject.SetActive(true);
        }
    }

    // Updates the value of the mouse sensitivity;
    private void UpdateMouseSensesitivty(float newValue)
    {
        mouseSensitivity = (int) newValue * 50;
    }

    // Attempts to clear all saved data
    private void DeleteDataAttempt()
    {
        StartCoroutine(ConfirmAction(DeleteData));
    }

    // Clears all saved data
    private void DeleteData()
    {
        SaveManager.ClearAllSaveData();
    }

    // Confirm the given action
    private IEnumerator ConfirmAction(Action func)
    {
        FindObjectOfType<ConfirmationManager>().OpenConfirmationMenu();

        yield return new WaitUntil(() => ConfirmationManager.hasDecided);

        if(ConfirmationManager.canContinue)
        {
            func.Invoke();
        }
    }
}
