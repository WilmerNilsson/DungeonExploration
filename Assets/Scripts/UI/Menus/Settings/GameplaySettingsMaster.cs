using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplaySettingsMaster : MonoBehaviour
{
    [SerializeField] private TMP_InputField gameSpeedInputField;

    GameManagerSO gameManager;
    
    void Start()
    {
        gameManager = GameManagerSO.Instance;
        SetInputValues();
    }

    void SetInputValues()
    {
        gameSpeedInputField.SetTextWithoutNotify((gameManager.GetTimeScale() * 100f).ToString());
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

}
