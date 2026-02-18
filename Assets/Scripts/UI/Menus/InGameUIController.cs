using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUIController : MonoBehaviour, IUIController
{
    [SerializeField] GameObject warningWindowForChangingScreen;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject inGameUI;


    private GameObject currentScreen;

    private GameManagerSO gameManager;

    private bool cutsceneWindowActive = false;
    private bool gameIsPaused = false;
    private bool canUnpause = true;
    private bool useWarningScreen = false;
    private GameObject heldWarningScreen;

    public event Action<bool> OnPauseChangeAction;
    public event Action OnChangeScreenAction;

#if DEBUG
    private void OnValidate()
    {
        if(warningWindowForChangingScreen == null)
        {
            Debug.LogWarning("warning screen not set", this);
        }
        if(pauseMenu == null)
        {
            Debug.LogWarning("pause menu not set", this);
        }
        if(inGameUI == null && gameObject.scene.rootCount != 0)
        {
            Debug.LogWarning("in game ui not set", this);
        }
    }
#endif

    void Awake()
    {
        
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
            if(currentScreen != null)
            {
                currentScreen.SetActive(false);
            }
            heldWarningScreen.SetActive(true);

            currentScreen = heldWarningScreen;
            if(OnChangeScreenAction != null)
            {
                OnChangeScreenAction();
            }
        }
    }

    public void TogglePauseMenu()
    {
        if(canUnpause)
        {
            if(gameIsPaused)
            {
                Resume();
                gameManager.SaveSettings();
            }
            else
            {
                Pause();
            }
            OnPauseChangeAction?.Invoke(gameIsPaused);
        }

        void Pause()
        {
            inGameUI.SetActive(false);
            pauseMenu.SetActive(true);
            gameManager.FreezeTime(true);
            gameManager.LockMouse(true);

            InvMaster.Instance.ClosePlayerInventory();

            gameIsPaused = true;
        }

        void Resume()
        {
            inGameUI.SetActive(true);
            pauseMenu.SetActive(false);

            gameManager.FreezeTime(false);
            gameManager.LockMouse(false);

            gameIsPaused = false;
        }
}

    public void ChangeCanUnpause(bool value)
    {
        canUnpause = value;
    }

    public void ChangeUseWarningScreen(bool value)
    {
        useWarningScreen = value;
    }

    public void ExitToMenu()
    {
        gameManager.SaveSettings();
        gameManager.FreezeTime(false);
        if(cutsceneWindowActive)
        {
            gameManager.FreezeTime(false);
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitToDesktop()
    {
        gameManager.SaveSettings();
        Application.Quit();
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
}
