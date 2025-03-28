// Golden Version
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZombieSpace;

// Managing level progression in a scene and HUD
// Dependencies: Save Manager, Target Manager, Settings Manager
// Main Contributors: Olivia Lazar
public class LevelManager : MonoBehaviour
{   
    public static int levelDifficulty;          // Scale modifier for level dependent attributes
    public static LevelState levelState;        // Whether the level can operate
    public static int gameScore;                // Combined score of all levels

    // Level Attributes
    [SerializeField, Range (1,4)]
    private int setLevelDifficulty = 1;

    // GUI Elements
    [SerializeField]
    private GameObject hudCanvas;
    private Text scoreTxt;
    private Text targetSafeTxt;
    private Text targetThreatenedTxt;
    private Text targetAttackedTxt;
    private Slider integritySldr;
    private Text targetCollectedTxt;
    private Text targetTxt;
    private Toggle collectedTgl;

    private Text lostTxt;
    private Text cannotEscapeTxt;
    private Text escapedTxt;

    // Sound Elements
    [SerializeField]
    private AudioClip lostSFX;
    [SerializeField]
    private AudioClip escapedSFX;

    private int levelScore = 0;                 // Points awarded in the current level
    private TargetState targetState;            // Status of target pickup
    private bool isCollected = false;           // Whether GUI has updated for collection state

    // Returns whether the level is running
    public static bool IsLevelActive()
    {
        return (levelState == LevelState.Active);
    }

    // Returns whether this is the first level
    public static bool IsFirstLevel()
    {
        return (SceneManager.GetActiveScene().buildIndex == 1);
    }

    // Awake is called before Start
    private void Awake()
    {
        levelDifficulty = setLevelDifficulty;
        levelState = LevelState.Active;
    }

    // Start is called before the first frame update
    private void Start()
    {
        Time.timeScale = 1;

        // Initialize target pickup state
        targetState = TargetManager.state;

        InitializeGUI();
        InitializeScore();
        UpdateScore(levelScore);
    }

    // Initialize connections to the HUD elements
    private void InitializeGUI()
    {
        Text[] hudText = hudCanvas.GetComponentsInChildren<Text>(true);
        scoreTxt = Array.Find(hudText, text => text.name == "Score Text");
        targetSafeTxt = Array.Find(hudText, text => text.name == "Safe Text");
        targetThreatenedTxt = Array.Find(hudText, text => text.name == "Threatened Text");
        targetAttackedTxt = Array.Find(hudText, text => text.name == "Attacked Text");
        targetCollectedTxt = Array.Find(hudText, text => text.name == "Collected Text");
        targetTxt = Array.Find(hudText, text => text.name == "Target Text");

        lostTxt = Array.Find(hudText, text => text.name == "Lost Text");
        cannotEscapeTxt = Array.Find(hudText, text => text.name == "Cannot Escape Text");
        escapedTxt = Array.Find(hudText, text => text.name == "Escaped Text");
        
        integritySldr = hudCanvas.GetComponentInChildren<Slider>(true);
        collectedTgl = hudCanvas.GetComponentInChildren<Toggle>(true);

        integritySldr.maxValue = TargetManager.integrity;
        integritySldr.value = TargetManager.integrity;
        targetTxt.text = TargetManager.itemName;

        UpdateTargetStateGUI();
    }

    // Initializes score based on level progress
    private void InitializeScore()
    {
        // Reset score on first level
        if(IsFirstLevel())
        {
            gameScore = 0;
        }

        // Load score if continued game
        if(SaveManager.IsLoadedLevel())
        {
            gameScore = SaveManager.LoadScore();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // While the level is active and the target pickup has not been collected
        if(IsLevelActive() && !isCollected)
        {
            // Update target pickup status
            if(targetState != TargetManager.state)
            {
                targetState = TargetManager.state;
                UpdateTargetStateGUI();
            }
            
            // Update target pickup integrity timer GUI if target pickup is being attacked
            if(targetState == TargetState.Attacked)
            {
                integritySldr.value = TargetManager.integrity;
            }
        }
    }

    // Updates target status GUI
    private void UpdateTargetStateGUI()
    {
        targetSafeTxt.gameObject.SetActive(false);
        targetThreatenedTxt.gameObject.SetActive(false);
        targetAttackedTxt.gameObject.SetActive(false);
        integritySldr.gameObject.SetActive(false);

        switch(targetState)
        {
            // If target pickup is safe
            case TargetState.Safe:
                targetSafeTxt.gameObject.SetActive(true);
                break;
            // If target pickup is being threatened
            case TargetState.Threatened:
                targetThreatenedTxt.gameObject.SetActive(true);
                break;
            // If target pickup is being attacked
            case TargetState.Attacked:
                targetAttackedTxt.gameObject.SetActive(true);
                integritySldr.gameObject.SetActive(true);
                break;
            // If target pickup has been collected
            case TargetState.Collected:
                targetCollectedTxt.gameObject.SetActive(true);
                collectedTgl.isOn = true;
                isCollected = true;
                break;
        }
    }

    // Initiates procedure for when current level is lost
    public void LevelLost(DefeatType reason)
    {
        levelState = LevelState.Over;
        
        // Select game over message based on given reason
        string message;
        switch(reason)
        {
            // If the target pickup is destroyed
            case DefeatType.TargetDestroyed:
                message = TargetManager.itemName + " was destroyed!";
                break;
            // If the player is killed
            case DefeatType.PlayerKilled:
                message = "You died!";
                break;
            default:
                message = "Undefined";
                break;
        }
        lostTxt.text = message;
        lostTxt.gameObject.SetActive(true);

        // Sound
        if(SettingsManager.isMusicOn) Camera.main.GetComponent<AudioSource>().mute = true;
        if(SettingsManager.isSFXOn) AudioSource.PlayClipAtPoint(lostSFX, Camera.main.transform.position);

        Invoke("ReloadLevel", 2);
    }

    // Loads the current level from the beginning 
    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Initiates an attempt to finish the current level
    public void LevelFinishAttempt()
    {
        // Level can only be finished if target pickup is collected
        if(targetState == TargetState.Collected)
        {
            LevelFinished();
        }
        else
        {
            StartCoroutine(FlashCannotEscapeText());
        }
    }

    // Show that the level cannot be finished yet
    private IEnumerator FlashCannotEscapeText()
    {
        cannotEscapeTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        cannotEscapeTxt.gameObject.SetActive(false);
    }

    // Initiates procedure for when current level is finished
    private void LevelFinished()
    {
        levelState = LevelState.Over;

        escapedTxt.gameObject.SetActive(true);

        // Sound
        if(SettingsManager.isMusicOn) Camera.main.GetComponent<AudioSource>().mute = true;
        if(SettingsManager.isSFXOn) AudioSource.PlayClipAtPoint(escapedSFX, Camera.main.transform.position);

        Invoke("LoadNextLevel", 2);
    }

    // Loads the next level
    private void LoadNextLevel()
    {
        gameScore += (levelScore + Mathf.RoundToInt(TargetManager.integrity));
        SaveManager.SaveProgress();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Updates the score and GUI by the given amount of points
    public void UpdateScore(int points)
    {
        levelScore += points;
        scoreTxt.text = levelScore.ToString();
    }
}
