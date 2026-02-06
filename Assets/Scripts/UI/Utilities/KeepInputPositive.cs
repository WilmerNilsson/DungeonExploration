using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class KeepInputPositive : MonoBehaviour
{
    private TMP_InputField numberInput;

    private void Awake()
    {
        numberInput = GetComponent<TMP_InputField>();
    }

    public void Validate(string txt)
    {
        if (txt.Length > 0 && txt[0] == '-') numberInput.text = txt.Remove(0, 1);
    }
}
