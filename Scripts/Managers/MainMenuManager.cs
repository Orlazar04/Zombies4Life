// Golden Version
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Managing the main menu
// Dependencies: Settings Manager, Save Manager, Confirmation Manager
// Main Contributors: Olivia Lazar
public class MainMenuManager : MonoBehaviour
{
    // Extras Panel
    [SerializeField]
    private GameObject extrasMenu;
    private GameObject window;

    private Button playBtn;
    private Button continueBtn;
    private Button settingsBtn;
    private Button extrasBtn;
    private Button quitBtn;

    // Start is called before the first frame update
    private void Start()
    {
        InitializeButtonsGUI();
        UpdateContinueButton();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateContinueButton();
    }

    // Initialize connections to the menu buttons
    private void InitializeButtonsGUI()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        playBtn = Array.Find(buttons, button => button.name == "Play Btn");
        continueBtn = Array.Find(buttons, button => button.name == "Continue Btn");
        settingsBtn = Array.Find(buttons, button => button.name == "Settings Btn");
        extrasBtn = Array.Find(buttons, button => button.name == "Extras Btn");
        quitBtn = Array.Find(buttons, button => button.name == "Quit Btn");

        playBtn.onClick.AddListener(StartGameAttempt);
        continueBtn.onClick.AddListener(ContinueGame);
        settingsBtn.onClick.AddListener(OpenSettingsMenu);
        extrasBtn.onClick.AddListener(OpenExtras);
        quitBtn.onClick.AddListener(QuitGameAttempt);
    }

    // Updates the active state of the continue button
    private void UpdateContinueButton()
    {
        continueBtn.interactable = SaveManager.DoesActiveGameExist();
    }

    // Attempts to start the game from the beginning
    private void StartGameAttempt()
    {
        if(SaveManager.DoesActiveGameExist())
        {
            StartCoroutine(ConfirmAction(StartGame));
        }
        else
        {
            StartGame();
        }
    }

    // Starts the game from the beginning
    private void StartGame()
    {
        SaveManager.NewGame();
        SceneManager.LoadScene(1);
    }

    // Starts the game from the saved state if one exists
    private void ContinueGame()
    {
        SceneManager.LoadScene(SaveManager.LoadLevel() + 1);
    }

    // Opens the settings menu
    private void OpenSettingsMenu()
    {
        FindObjectOfType<SettingsManager>().OpenSettingsMenu();
    }

    // Opens the extras panel
    private void OpenExtras()
    {
        window = Instantiate(extrasMenu);

        Button exitBtn = window.GetComponentInChildren<Button>(true);
        exitBtn.onClick.AddListener(CloseExtras);
    }

    // Closes the extras panel
    private void CloseExtras()
    {
        Destroy(window);
    }

    // Attempts to close the game
    private void QuitGameAttempt()
    {
        StartCoroutine(ConfirmAction(QuitGame));
    }

    // Closes the game
    private void QuitGame()
    {
        Application.Quit();
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
