// Golden Version
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Managing the menu that ensures a substantial button press was wanted
// Main Contributors: Olivia Lazar
public class ConfirmationManager : MonoBehaviour
{
    public static bool hasDecided;
    public static bool canContinue;

    [SerializeField]
    private GameObject confirmationMenu;

    private GameObject window;
    private Button yesBtn;
    private Button noBtn;

    // Opens the confirmation menu
    public void OpenConfirmationMenu()
    {
        window = Instantiate(confirmationMenu);
        hasDecided = false;
        InitializeGUI();
    }

    // Initialize connections to the GUI components
    private void InitializeGUI()
    {
        Button[] buttons = window.GetComponentsInChildren<Button>(true);
        yesBtn = Array.Find(buttons, button => button.name == "Yes Btn");
        noBtn = Array.Find(buttons, button => button.name == "No Btn");

        yesBtn.onClick.AddListener(ConfirmAction);
        noBtn.onClick.AddListener(DenyAction);
    }

    // Confirm the paused action
    public void ConfirmAction()
    {
        canContinue = true;
        hasDecided = true;
        Destroy(window);
    }

    // Deny the paused action
    public void DenyAction()
    {
        canContinue = false;
        hasDecided = true;
        Destroy(window);
    }
}
