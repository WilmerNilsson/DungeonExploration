using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AudioInputPair
{
    public TMP_InputField inputField; public Slider slider;
}

public class AudioInputSettingsMaster : MonoBehaviour
{
    [SerializeField] GameObject masterAudioParent;
    AudioInputPair masterAudioPair = new AudioInputPair();
    [SerializeField] GameObject effectsAudioParent;
    AudioInputPair effectsAudioPair = new AudioInputPair();
    [SerializeField] GameObject musicAudioParent;
    AudioInputPair musicAudioPair = new AudioInputPair();

    [SerializeField] float overNewValueWarningThreshhold = 100f;

    [SerializeField] GameObject warningWindow;
    AudioInputPair currentWarningPair;
    float currentWarningAudioLevel;

    GameManagerSO gameManager;
    IUIController uIController;

    private void Awake()
    {
        masterAudioPair.inputField = masterAudioParent.GetComponentInChildren<TMP_InputField>();
        masterAudioPair.slider = masterAudioParent.GetComponentInChildren<Slider>();

        effectsAudioPair.inputField = effectsAudioParent.GetComponentInChildren<TMP_InputField>();
        effectsAudioPair.slider = effectsAudioParent.GetComponentInChildren<Slider>();

        musicAudioPair.inputField = musicAudioParent.GetComponentInChildren<TMP_InputField>();
        musicAudioPair.slider = musicAudioParent.GetComponentInChildren<Slider>();
    }

    private void Start() 
    {
        gameManager = GameManagerSO.GetGameManagerSOInstance();
        uIController = GameObject.FindGameObjectWithTag("MainUI").GetComponent<IUIController>();

        masterAudioPair.inputField.SetTextWithoutNotify(gameManager.GetMasterVolume().ToString());
        masterAudioPair.slider.SetValueWithoutNotify(gameManager.GetMasterVolume());

        effectsAudioPair.inputField.SetTextWithoutNotify(gameManager.GetEffectsVolume().ToString());
        effectsAudioPair.slider.SetValueWithoutNotify(gameManager.GetEffectsVolume());

        musicAudioPair.inputField.SetTextWithoutNotify(gameManager.GetMusicVolume().ToString());
        musicAudioPair.slider.SetValueWithoutNotify(gameManager.GetMusicVolume());
    }

    public void WarningWindowAnswer(bool wantToChange)
    {
        if(wantToChange == true)
        {
            currentWarningPair.slider.SetValueWithoutNotify(currentWarningAudioLevel);
            if(currentWarningPair == masterAudioPair)
            {
                gameManager.SetMasterVolume(currentWarningAudioLevel);
            }
            else if(currentWarningPair == musicAudioPair)
            {
                gameManager.SetMusicVolume(currentWarningAudioLevel);
            }
            else if(currentWarningPair == effectsAudioPair)
            {
                gameManager.SetEffectsVolume(currentWarningAudioLevel);
            }
        }
        else
        {
            currentWarningPair.inputField.SetTextWithoutNotify(currentWarningPair.slider.value.ToString());
        }

        warningWindow.SetActive(false);
        uIController.ChangeCanUnpause(true);
    }

    void ActivateSoundWarning()
    {
        uIController.ChangeCanUnpause(false);
        warningWindow.SetActive(true);
    }

    public void ChangeMasterAudioByString(string newValueString)
    {
        float newValue = float.Parse(newValueString);

        if(newValue > overNewValueWarningThreshhold)
        {
            currentWarningPair = masterAudioPair;
            currentWarningAudioLevel = newValue;
            ActivateSoundWarning();
        }
        else
        {
            gameManager.SetMasterVolume(newValue);
            masterAudioPair.slider.SetValueWithoutNotify(newValue);
        }
    }

    public void ChangeMasterAudioByFloat(float newValue)
    {
        gameManager.SetMasterVolume(newValue);
        masterAudioPair.inputField.SetTextWithoutNotify(newValue.ToString());
    }

    public void ChangeEffectsAudioValueByString(string newValueString)
    {
        float newValue = float.Parse(newValueString);

        if(newValue > overNewValueWarningThreshhold)
        {
            currentWarningPair = effectsAudioPair;
            currentWarningAudioLevel = newValue;
            ActivateSoundWarning();
        }
        else
        {
            gameManager.SetEffectsVolume(newValue);
            effectsAudioPair.slider.SetValueWithoutNotify(newValue);
        }
    }

    public void ChangeEffectsAudioByFloat(float newValue)
    {
        gameManager.SetEffectsVolume(newValue);
        effectsAudioPair.inputField.SetTextWithoutNotify(newValue.ToString());
    }

    public void ChangeMusicAudioValueByString(string newValueString)
    {
        float newValue = float.Parse(newValueString);

        if(newValue > overNewValueWarningThreshhold)
        {
            currentWarningPair = musicAudioPair;
            currentWarningAudioLevel = newValue;
            ActivateSoundWarning();
        }
        else
        {
            gameManager.SetMusicVolume(newValue);
            musicAudioPair.slider.SetValueWithoutNotify(newValue);
        }
    }

    public void ChangeMusicAudioByFloat(float newValue)
    {
        gameManager.SetMusicVolume(newValue);
        musicAudioPair.inputField.SetTextWithoutNotify(newValue.ToString());
    }
}
