// Golden Version
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Managing the game once over
// Dependencies: Save Manager
// Main Contributors: Olivia Lazar
public class EndGameManager : MonoBehaviour
{
    private Button exitBtn;
    private Button quitBtn;
    private Text finalScoreTxt;
    private Text highScoreTxt;
    private Text newHighTxt;

    // Start is called before the first frame update
    private void Start()
    {
        InitializeButtonsGUI();
        InitializeScoreMessage();

        // Unlock cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SaveManager.SaveFinishedGame();
    }

    // Initialize connections to the menu buttons
    private void InitializeButtonsGUI()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        exitBtn = Array.Find(buttons, button => button.name == "Exit Btn");
        quitBtn = Array.Find(buttons, button => button.name == "Quit Btn");

        exitBtn.onClick.AddListener(ReturnToMainMenu);
        quitBtn.onClick.AddListener(QuitGameAttempt);
    }

    // Initializes the GUI for final and high scores
    private void InitializeScoreMessage()
    {
        Text[] texts = GetComponentsInChildren<Text>(true);
        finalScoreTxt = Array.Find(texts, text => text.name == "Final Score Txt");
        highScoreTxt = Array.Find(texts, text => text.name == "High Score Txt");
        newHighTxt = Array.Find(texts, text => text.name == "New High Txt");

        string finalScore = SaveManager.GetFinalScore().ToString();
        finalScoreTxt.text = finalScore;

        // Set high score based on whether a new high score is acheived
        if(SaveManager.IsNewHighScore())
        {
            highScoreTxt.text = finalScore;
            StartCoroutine(FlashNewHighScore());
        }
        else
        {
            highScoreTxt.text = SaveManager.GetHighScore().ToString();
        }
    }

    // Flashes that a new high score was acheived
    private IEnumerator FlashNewHighScore()
    {
        newHighTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        newHighTxt.gameObject.SetActive(false);
    }

    // Return to the main menu
    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
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
