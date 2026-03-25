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

    [SerializeField] GameObject warningWindow;
    AudioInputPair currentWarningPair;
    float currentWarningAudioLevel;

    GameManagerSO gameManager;

    private void Start() 
    {
        gameManager = GameManagerSO.Instance;

        masterAudioPair.InputField?.SetTextWithoutNotify(gameManager.GetMasterVolume().ToString());
        masterAudioPair.Slider?.SetValueWithoutNotify(gameManager.GetMasterVolume());

        effectsAudioPair.InputField?.SetTextWithoutNotify(gameManager.GetEffectsVolume().ToString());
        effectsAudioPair.Slider?.SetValueWithoutNotify(gameManager.GetEffectsVolume());

        musicAudioPair.InputField?.SetTextWithoutNotify(gameManager.GetMusicVolume().ToString());
        musicAudioPair.Slider?.SetValueWithoutNotify(gameManager.GetMusicVolume());
    }
    public void ChangeMasterAudioByString(string newValueString)
    {
        float newValue = float.Parse(newValueString);
        
        gameManager.SetMasterVolume(newValue);
        masterAudioPair.Slider?.SetValueWithoutNotify(newValue);
        
    }

    public void ChangeMasterAudioByFloat(float newValue)
    {
        gameManager.SetMasterVolume(newValue);
        masterAudioPair.InputField?.SetTextWithoutNotify(newValue.ToString());
    }

    public void ChangeEffectsAudioValueByString(string newValueString)
    {
        float newValue = float.Parse(newValueString);
        
        gameManager.SetEffectsVolume(newValue);
        effectsAudioPair.Slider?.SetValueWithoutNotify(newValue);
    }

    public void ChangeEffectsAudioByFloat(float newValue)
    {
        gameManager.SetEffectsVolume(newValue);
        effectsAudioPair.InputField?.SetTextWithoutNotify(newValue.ToString());
    }

    public void ChangeMusicAudioValueByString(string newValueString)
    {
        float newValue = float.Parse(newValueString);
        
        gameManager.SetMusicVolume(newValue);
        musicAudioPair.Slider?.SetValueWithoutNotify(newValue);
    }

    public void ChangeMusicAudioByFloat(float newValue)
    {
        gameManager.SetMusicVolume(newValue);
        musicAudioPair.InputField?  .SetTextWithoutNotify(newValue.ToString());
    }
}
