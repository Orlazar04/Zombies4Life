// Golden Version
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using ZombieSpace;

// Managing the pause menu
// Dependencies: Level State, Settings Manager, Save Manager, Confirmation Manager
// Main Contributors: Olivia Lazar
public class PauseMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenu;

    private Button resumeBtn;
    private Button settingsBtn;
    private Button exitBtn;

    private Text levelTxt;
    private Text scoreTxt;

    private PostProcessVolume renderingFX;     // Camera effect on level behind the menu

    // Start is called before the first frame update
    private void Start()
    {
        InitializeButtonsGUI();
        InitializeTextGUI();

        // Initialize camera effects
        renderingFX = Camera.main.gameObject.GetComponent<PostProcessVolume>();
        renderingFX.enabled = false;

        LockCursor();
    }

    // Initialize connections to the menu buttons
    private void InitializeButtonsGUI()
    {
        Button[] buttons = pauseMenu.GetComponentsInChildren<Button>(true);
        resumeBtn = Array.Find(buttons, button => button.name == "Resume Btn");
        settingsBtn = Array.Find(buttons, button => button.name == "Settings Btn");
        exitBtn = Array.Find(buttons, button => button.name == "Exit Btn");

        resumeBtn.onClick.AddListener(UnpauseLevel);
        settingsBtn.onClick.AddListener(OpenSettingsMenu);
        exitBtn.onClick.AddListener(ExitLevel);
    }

    // Initialize connections to the stat text
    private void InitializeTextGUI()
    {
        Text[] txt = pauseMenu.GetComponentsInChildren<Text>(true);
        levelTxt = Array.Find(txt, text => text.name == "Level Txt");
        scoreTxt = Array.Find(txt, text => text.name == "Score Txt");

        levelTxt.text = SceneManager.GetActiveScene().buildIndex.ToString() + " / 5";
        scoreTxt.text = LevelManager.gameScore.ToString();
    }

    // Update is called once per frame
    private void Update()
    {
        // Pause an active level when exit button is pressed
        if(LevelManager.IsLevelActive() && Input.GetButton("Cancel"))
        {
            PauseLevel();
        }
    }

    // Pauses the game
    private void PauseLevel()
    {
        LevelManager.levelState = LevelState.Paused;
        Camera.main.GetComponent<AudioSource>().Pause();
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        renderingFX.enabled = true;
        UnlockCursor();
    }

    // Unpauses the game
    private void UnpauseLevel()
    {
        LockCursor();
        pauseMenu.SetActive(false);
        renderingFX.enabled = false;
        Time.timeScale = 1;
        Camera.main.GetComponent<AudioSource>().UnPause();
        LevelManager.levelState = LevelState.Active;
    }

    // Opens the settings menu
    private void OpenSettingsMenu()
    {
        FindObjectOfType<SettingsManager>().OpenSettingsMenu();
    }

    // Attempts to exit to the main menu
    private void ExitLevelAttempt()
    {
        StartCoroutine(ConfirmAction(ExitLevel));
    }

    // Exits to the main menu
    private void ExitLevel()
    {
        Time.timeScale = 1;
        Camera.main.GetComponent<AudioSource>().UnPause();
        SceneManager.LoadScene(0);
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

    // Locks the cursor for gameplay
    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Unlocks the cursor for menus
    private void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
