using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class AudioInputPair
{
    public TMP_InputField InputField; public Slider Slider;
}

public class AudioInputSettingsMaster : MonoBehaviour
{
    [SerializeField] private AudioInputPair masterAudioPair;
    [SerializeField] private AudioInputPair effectsAudioPair;
    [SerializeField] private AudioInputPair musicAudioPair;

    [SerializeField] float overNewValueWarningThreshhold = 100f;

    [SerializeField] GameObject warningWindow;
    AudioInputPair currentWarningPair;
    float currentWarningAudioLevel;

    GameManagerSO gameManager;
    IUIController uIController;

    private void Start() 
    {
        gameManager = GameManagerSO.Instance;
        uIController = GameObject.FindGameObjectWithTag("MainUI").GetComponent<IUIController>();

        masterAudioPair.InputField?.SetTextWithoutNotify(gameManager.GetMasterVolume().ToString());
        masterAudioPair.Slider?.SetValueWithoutNotify(gameManager.GetMasterVolume());

        effectsAudioPair.InputField?.SetTextWithoutNotify(gameManager.GetEffectsVolume().ToString());
        effectsAudioPair.Slider?.SetValueWithoutNotify(gameManager.GetEffectsVolume());

        musicAudioPair.InputField?.SetTextWithoutNotify(gameManager.GetMusicVolume().ToString());
        musicAudioPair.Slider?.SetValueWithoutNotify(gameManager.GetMusicVolume());
    }

    //untested since we can't put it above 100 rn
    public void WarningWindowAnswer(bool wantToChange)
    {
        if(wantToChange == true)
        {
            currentWarningPair.Slider.SetValueWithoutNotify(currentWarningAudioLevel);
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
            currentWarningPair.InputField?.SetTextWithoutNotify(currentWarningPair.Slider.value.ToString());
        }

        warningWindow.SetActive(false);
        uIController.ChangeCanUnpause(true);
    }

    private void ActivateSoundWarning()
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
            masterAudioPair.Slider?.SetValueWithoutNotify(newValue);
        }
    }

    public void ChangeMasterAudioByFloat(float newValue)
    {
        gameManager.SetMasterVolume(newValue);
        masterAudioPair.InputField?.SetTextWithoutNotify(newValue.ToString());
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
            effectsAudioPair.Slider?.SetValueWithoutNotify(newValue);
        }
    }

    public void ChangeEffectsAudioByFloat(float newValue)
    {
        gameManager.SetEffectsVolume(newValue);
        effectsAudioPair.InputField?.SetTextWithoutNotify(newValue.ToString());
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
            musicAudioPair.Slider?.SetValueWithoutNotify(newValue);
        }
    }

    public void ChangeMusicAudioByFloat(float newValue)
    {
        gameManager.SetMusicVolume(newValue);
        musicAudioPair.InputField?  .SetTextWithoutNotify(newValue.ToString());
    }
}
