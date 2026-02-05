using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using TMPro;

public class InGameUIController : MonoBehaviour, IUIController
{
    [Serializable]
    private class AbilityCountdown
    {
        public int attackID = 0;

        [SerializeField] TMP_Text textA;

        public int selfCooldown = 0;
        public int globalCooldown = 0;
        public bool isInSelfCooldown = false;
        public bool isInGlobalCooldown = false;

        IEnumerator SelfCooldown()
        {
            isInSelfCooldown = true;
            UpdateCounter();
            while(selfCooldown > 0)
            {
                yield return new WaitForSeconds(0.1f);
                selfCooldown --;
                UpdateCounter();
            }
            isInSelfCooldown = false;
        }

        IEnumerator GlobalCooldown()
        {
            isInGlobalCooldown = true;
            UpdateCounter();
            while(globalCooldown > 0)
            {
                yield return new WaitForSeconds(0.1f);
                globalCooldown --;
                UpdateCounter();
            }
            isInGlobalCooldown = false;
        }

        public void UpdateCounter()
        {
            string cooldownString;
            if(globalCooldown == 0 && selfCooldown == 0)
            {
                textA.SetText("");
            }
            else if(selfCooldown > globalCooldown)
            {
                globalCooldown = 0;

                cooldownString = selfCooldown.ToString();

                if(selfCooldown >= 100)
                {
                    cooldownString = (selfCooldown / 10).ToString();
                    textA.SetText(cooldownString);
                }
                else if(selfCooldown >= 10)
                {
                    cooldownString = selfCooldown.ToString();
                    textA.SetText(cooldownString.Insert(1, "."));
                }
                else
                {
                    cooldownString = selfCooldown.ToString();
                    textA.SetText(cooldownString.Insert(0, "0."));
                }
            }
            else
            {
                selfCooldown = 0;

                cooldownString = globalCooldown.ToString();

                if(globalCooldown >= 100)
                {
                    cooldownString = (globalCooldown / 10).ToString();
                    textA.SetText(cooldownString);
                }
                else if(globalCooldown >= 10)
                {
                    cooldownString = globalCooldown.ToString();
                    textA.SetText(cooldownString.Insert(1, "."));
                }
                else
                {
                    cooldownString = globalCooldown.ToString();
                    textA.SetText(cooldownString.Insert(0, "0."));
                }
            }
        }
    }

    [SerializeField] GameObject warningWindowForChangingScreen;

    [SerializeField] List<AbilityCountdown> abilityCountdowns;
    GameObject currentScreen;

    GameManagerSO gameManager;

    bool cutsceneWindowActive = false;
    bool gameIsPaused = false;
    bool canUnpause = true;
    bool useWarningScreen = false;
    GameObject heldWarningScreen;

    GameObject cutsceneCanvas;
    GameObject pauseMenu;
    GameObject inGameUI;

    public event Action<bool> OnPauseChangeAction;
    public event Action OnChangeScreenAction;

    void Awake()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterMovement>().OnAttackAction += AttackSorter;

        inGameUI = transform.GetChild(0).gameObject;
        cutsceneCanvas = transform.GetChild(1).gameObject;
        pauseMenu = transform.GetChild(2).gameObject;
    }

    private void Start()
    {
        gameManager = GameManagerSO.GetGameManagerSOInstance();
    }

    void AttackSorter(int iDvalue, float selfCooldownValue, float globalCooldownValue)
    {
        for(int i = 0; i < abilityCountdowns.Count; i++)
        {
            if(iDvalue == abilityCountdowns[i].attackID)
            {
                abilityCountdowns[i].selfCooldown = (int) (selfCooldownValue * 10);
            }
            abilityCountdowns[i].globalCooldown = (int) (globalCooldownValue * 10);

            if((abilityCountdowns[i].attackID != iDvalue || globalCooldownValue > selfCooldownValue) && !abilityCountdowns[i].isInGlobalCooldown)
            {
                StartCoroutine(GlobalCooldown(i));
            }
            else if(!abilityCountdowns[i].isInSelfCooldown)
            {
                StartCoroutine(SelfCooldown(i));
            }
        }

        IEnumerator SelfCooldown(int i)
        {
            abilityCountdowns[i].isInSelfCooldown = true;
            abilityCountdowns[i].UpdateCounter();
            while(abilityCountdowns[i].selfCooldown > 0)
            {
                yield return new WaitForSeconds(0.1f);
                abilityCountdowns[i].selfCooldown --;
                abilityCountdowns[i].UpdateCounter();
            }
            abilityCountdowns[i].isInSelfCooldown = false;
        }

        IEnumerator GlobalCooldown(int i)
        {
            abilityCountdowns[i].isInGlobalCooldown = true;
            abilityCountdowns[i].UpdateCounter();
            while(abilityCountdowns[i].globalCooldown > 0)
            {
                yield return new WaitForSeconds(0.1f);
                abilityCountdowns[i].globalCooldown --;
                abilityCountdowns[i].UpdateCounter();
            }
            abilityCountdowns[i].isInGlobalCooldown = false;
        }
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
            if(OnPauseChangeAction != null)
            {
                OnPauseChangeAction(gameIsPaused);
            }
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

    public void ActivateCutsceneWindow(bool value)
    {
        cutsceneWindowActive = value;
        cutsceneCanvas.SetActive(value);

        gameManager.FreezeTime(value);
    }

    void Pause()
    {
        inGameUI.SetActive(false);
        pauseMenu.SetActive(true);
        gameManager.FreezeTime(true);
        gameIsPaused = true;
    }

    public void Resume()
    {
        inGameUI.SetActive(true);
        pauseMenu.SetActive(false);

        gameManager.FreezeTime(false);
        
        gameIsPaused = false;
        if(OnPauseChangeAction != null)
        {
            OnPauseChangeAction(gameIsPaused);
        }
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
