using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplaySettingsMaster : MonoBehaviour
{
    [SerializeField] private TMP_InputField gameSpeedInputField;
    [SerializeField] private TMP_InputField playerHPInputField;
    [SerializeField] private TMP_InputField enemyHPInputField;

    GameManagerSO gameManager;
    
    void Start()
    {
        gameManager = GameManagerSO.GetGameManagerSOInstance();
        SetInputValues();
    }

    void SetInputValues()
    {
        gameSpeedInputField.SetTextWithoutNotify((gameManager.GetTimeScale() * 100f).ToString());
        playerHPInputField.SetTextWithoutNotify((gameManager.GetPlayerHealthCheatValue() * 100f).ToString());
        enemyHPInputField.SetTextWithoutNotify((gameManager.GetEnemyHealthCheatValue() * 100f).ToString());
    }

    public void ChangeGameSpeed(string newTimeScaleString)
    {
        float newTimeScale = float.Parse(newTimeScaleString);

        if(newTimeScale < 10f)
        {
            newTimeScale = 10f;
            gameSpeedInputField.SetTextWithoutNotify("10");
        }
        gameManager.SetTimeScale(newTimeScale / 100);
    }

    public void ChangePlayerHPCheat(string newMultiplierString)
    {
        float newMultiplier = float.Parse(newMultiplierString);

        if(newMultiplier < 1f)
        {
            newMultiplier = 1f;
            playerHPInputField.SetTextWithoutNotify("1");
        }
        gameManager.SetPlayerHealthCheatValue(newMultiplier / 100);
    }

    public void ChangeEnemyHPCheat(string newMultiplierString)
    {
        float newMultiplier = float.Parse(newMultiplierString);

        if(newMultiplier < 1f)
        {
            newMultiplier = 1f;
            enemyHPInputField.SetTextWithoutNotify("1");
        }
        gameManager.SetEnemyHealthCheatValue(newMultiplier / 100);
    }

}
