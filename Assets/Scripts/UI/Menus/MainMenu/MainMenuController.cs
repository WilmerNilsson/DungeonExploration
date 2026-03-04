using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour, IUIController
{
    [SerializeField] GameObject warningWindowForChangingScreen;
    GameObject nonTitleScreen;
    GameObject currentScreen;

    bool useWarningScreen = false;
    GameObject heldWarningScreen;

    GameManagerSO gameManager;

    public event Action OnChangeScreenAction;

    private void Awake()
    {
        nonTitleScreen = transform.GetChild(2).gameObject;
    }

    private void Start()
    {
        gameManager = GameManagerSO.Instance;
    }

    public void WarningWindowAnswer(bool answer)
    {
        warningWindowForChangingScreen.SetActive(false);

        if(answer)
        {
            currentScreen.SetActive(false);
            heldWarningScreen.SetActive(true);

            currentScreen = heldWarningScreen;
            if(OnChangeScreenAction != null)
            {
                OnChangeScreenAction();
            }
        }
    }

    public void ExitToDesktop()
    {
        gameManager.SavefileManager.SaveSettings();
        Application.Quit();
    }

    public void ToggleNonTitleScreen()
    {
        gameManager.SavefileManager.SaveSettings();
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
