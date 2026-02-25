using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SavesMaster : MonoBehaviour
{
    [SerializeField] Button playButton;
    private TMP_Text playButtonText;
    [SerializeField] Button deleteButton;
    private TMP_Text deleteButtonText;
    [SerializeField] GameObject warningWindow;

    private int selectedSaveFileInt = 0;
    private TMP_Text selectedSaveFileButtonText;

    private GameManagerSO gameManager;

    private void Awake()
    {
        playButtonText = playButton.GetComponentInChildren<TMP_Text>();
        deleteButtonText = deleteButton.GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        gameManager = GameManagerSO.Instance;
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
        gameManager.SavefileManager.PlaySavefile(selectedSaveFileInt);
    }

    public void ShowDeleteWarningWindow()
    {
        warningWindow.SetActive(true);
    }

    public void WarningWindowAnswer(bool wantToDelete)
    {
        if(wantToDelete)
        {
            gameManager.SavefileManager.DeleteSavefile(selectedSaveFileInt);
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
