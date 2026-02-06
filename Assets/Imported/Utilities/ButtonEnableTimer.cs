using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEnableTimer : MonoBehaviour
{
    [SerializeField] float waitTime = 2f;

    Button button;

    private void Awake() 
    {
        button = GetComponent<Button>();
    }

    private void OnEnable() 
    {
        button.interactable = false;
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        yield return new WaitForSecondsRealtime(waitTime);
        button.interactable = true;
    }

}
