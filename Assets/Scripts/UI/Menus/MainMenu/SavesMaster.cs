using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SavesMaster : MonoBehaviour
{
    [SerializeField] private string defaultSceneName;

    [SerializeField] private Button playButton;
    private TMP_Text playButtonText;
    [SerializeField] private Button deleteButton;
    private TMP_Text deleteButtonText;
    [SerializeField] private GameObject warningWindow;
    [SerializeField] private TMP_Text[] orderedSaveTexts;
    [SerializeField] private string emptySaveString = string.Empty;

    private int selectedSaveFileInt = 0;
    //private TMP_Text selectedSaveFileButtonText;

    private void Awake()
    {
        playButtonText = playButton.GetComponentInChildren<TMP_Text>();
        deleteButtonText = deleteButton.GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        //+1 cause it starts counting from 1
        for (int i = 1; i <= orderedSaveTexts.Length; i++)
        {
            SavefileData savefileData = GameManagerSO.Instance.SavefileManager.ReadSaveFileNoCreate(i);

            if (savefileData != null)
            {
                if(savefileData.PlayerSaveData != null)
                {
                    orderedSaveTexts[i].text = savefileData.PlayerSaveData.RunCount.ToString() + " runs";
                }
                else
                {
                    orderedSaveTexts[i].text = "0 runs";
                }
            }
            else
            {
                orderedSaveTexts[i].text = emptySaveString;
            }
        }
    }

    public void SelectSavefile(int newSaveFileNr)
    {
        playButton.interactable = true;
        deleteButton.interactable = true;

        if(newSaveFileNr != selectedSaveFileInt)
        {
            //if(newSaveFileButtonText != null)
            //{
            //    selectedSaveFileButtonText.color = normalSaveColor;
            //}

            selectedSaveFileInt = newSaveFileNr;
            //selectedSaveFileButtonText = newSaveFileButtonText;

            //selectedSaveFileButtonText.color = selectedSaveColor;
        }
    }

    public void PlaySelectedSaveFile()
    {
        GameManagerSO.Instance.SavefileManager.PlaySavefile(selectedSaveFileInt, defaultSceneName);
    }

    public void ShowDeleteWarningWindow()
    {
        warningWindow.SetActive(true);
    }

    public void WarningWindowAnswer(bool wantToDelete)
    {
        if(wantToDelete)
        {
            GameManagerSO.Instance.SavefileManager.DeleteSavefile(selectedSaveFileInt);

            orderedSaveTexts[selectedSaveFileInt].text = emptySaveString;

            DeselectButton();
        }

        warningWindow.SetActive(false);
    }

    private void DeselectButton()
    {
        playButton.interactable = false;
        deleteButton.interactable = false;

        //selectedSaveFileButtonText.color = normalSaveColor;
        selectedSaveFileInt = 0;
    }
    
}
