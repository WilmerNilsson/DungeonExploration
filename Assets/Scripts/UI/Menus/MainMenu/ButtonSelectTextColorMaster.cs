using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonSelectTextColorMaster : MonoBehaviour
{
    [SerializeField] Color selectedColor = Color.cyan;
    [SerializeField] Color standardColor = Color.white;

    TMP_Text selectedText;

    public void ChangeSelectedButtonText(TMP_Text newText)
    {
        if(selectedText != newText)
            {
                if(selectedText != null)
                {
                    selectedText.color = standardColor;
                }

                selectedText = newText;
                selectedText.color = selectedColor;
            }
    }

    public void DeselectText()
    {
        if(selectedText != null)
        {
            selectedText.color = standardColor;
            selectedText = null;
        }
    }
}
