using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour, IUIController
{
    [SerializeField] private GameObject warningWindowForChangingScreen;
    [SerializeField] private GameObject nonTitleScreen;
#nullable enable
    private GameObject? currentScreen;

    private bool useWarningScreen = false;
    private GameObject? heldWarningScreen;

    public event Action? OnChangeScreenAction;

    public void WarningWindowAnswer(bool answer)
    {
        warningWindowForChangingScreen.SetActive(false);

        if(answer)
        {
            currentScreen?.SetActive(false);
            heldWarningScreen?.SetActive(true);

            currentScreen = heldWarningScreen;
            if(OnChangeScreenAction != null)
            {
                OnChangeScreenAction();
            }
        }
    }

    public void ExitToDesktop()
    {
        GameManagerSO.Instance.SavefileManager.SaveSettings();
        Application.Quit();
    }

    public void ToggleNonTitleScreen()
    {
        GameManagerSO.Instance.SavefileManager.SaveSettings();
        nonTitleScreen.SetActive(!nonTitleScreen.activeSelf);
    }

    public void GoToScreen(GameObject newScreen)
    {
        if(currentScreen != newScreen)
        {
            if(useWarningScreen)
            {
                heldWarningScreen = newScreen;
                warningWindowForChangingScreen.SetActive(true);
            }
            else
            {
                if(currentScreen != null)
                {
                    currentScreen.SetActive(false);
                }
                newScreen.SetActive(true);

                currentScreen = newScreen;
                if(OnChangeScreenAction != null)
                {
                    OnChangeScreenAction();
                }
            }
        }
    }

    public void ChangeCanUnpause(bool canUnpause)
    {
        
    }

    public void TogglePauseMenu()
    {
        
    }

    public void ChangeUseWarningScreen(bool value)
    {
        useWarningScreen = value;
    }
}
