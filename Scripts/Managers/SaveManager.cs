// Golden Version
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZombieSpace;

// This script is meant for saving the player's progress
// Dependencies: Level Manager, Weapon Manager, Player Health
// Main Contributors: Olivia Lazar
public class SaveManager : MonoBehaviour
{
    // Game Activity
    // 0: No Active Game - Can only start new game
    // 1: Active Game - Save data exists and can use continue button
    // 2: Load mode - Replaces values with saved ones

    // Returns whether there is an active game that can be loaded
    public static bool DoesActiveGameExist()
    {
        return (PlayerPrefs.GetInt("Game Activity") != 0);
    }

    // Returns whether the game is in load mode
    public static bool IsLoadedLevel()
    {
        return (PlayerPrefs.GetInt("Game Activity") == 2);
    }

    // Clears the saved data for a new game
    public static void NewGame()
    {
        PlayerPrefs.SetInt("Game Activity", 0);

        PlayerPrefs.SetInt("Level", 0);
        PlayerPrefs.SetInt("Game Score", 0);

        PlayerPrefs.SetInt("Melee Weapon", (int) WeaponName.None);
        PlayerPrefs.SetInt("Ranged Weapon", (int) WeaponName.None);
        PlayerPrefs.SetInt("Ammo", 0);
        PlayerPrefs.SetInt("Armor", 0);
    }

    // Saves the level, score, and weapon info at the end of a leve
    public static void SaveProgress()
    {
        if(PlayerPrefs.GetInt("Game Activity") != 1)
        {
            PlayerPrefs.SetInt("Game Activity", 1);
        }

        PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.SetInt("Game Score", LevelManager.gameScore);

        PlayerPrefs.SetInt("Melee Weapon", WeaponManager.meleeIndex);
        PlayerPrefs.SetInt("Ranged Weapon", WeaponManager.rangedIndex);
        PlayerPrefs.SetInt("Ammo", WeaponManager.ammo);
        PlayerPrefs.SetInt("Armor", PlayerHealth.protection);
    }

    // Loads the saved level and iniates loading values
    public static int LoadLevel()
    {
        PlayerPrefs.SetInt("Game Activity", 2);
        return PlayerPrefs.GetInt("Level");
    }

    // Loads the score
    public static int LoadScore()
    {
        return PlayerPrefs.GetInt("Game Score");
    }

    // Loads the weapons
    public static int[] LoadWeapons()
    {
        int[] weapons = new int[3];
        weapons[0] = PlayerPrefs.GetInt("Melee Weapon");
        weapons[1] = PlayerPrefs.GetInt("Ranged Weapon");
        weapons[2] = PlayerPrefs.GetInt("Ammo Weapon");
        return weapons;
    }

    // Loads the armor
    public static int LoadArmor()
    {
        return PlayerPrefs.GetInt("Armor");
    }

    // Updates the saved data to a finished game state
    public static void SaveFinishedGame()
    {
        PlayerPrefs.SetInt("Game Activity", 0);

        if(!PlayerPrefs.HasKey("High Score"))
        {
            PlayerPrefs.SetInt("High Score", 0);
        }

        if(IsNewHighScore())
        {
            PlayerPrefs.SetInt("High Score", PlayerPrefs.GetInt("Game Score"));
        }
    }

    // Whether a new highscore has been made
    public static bool IsNewHighScore()
    {
        return (PlayerPrefs.GetInt("Game Score") > PlayerPrefs.GetInt("High Score"));
    }

    // Returns the value of the final game score
    public static int GetFinalScore()
    {
        return PlayerPrefs.GetInt("Game Score");
    }

    // Returns the value of the current high score
    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt("High Score");
    }

    // Deletes all saved data
    public static void ClearAllSaveData()
    {
        PlayerPrefs.DeleteAll();
    }
}
