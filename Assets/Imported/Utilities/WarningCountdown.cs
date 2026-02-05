using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WarningCountdown : MonoBehaviour
{
    [SerializeField] GraphicsSettings graphicsScript;
    [SerializeField] int countdownMaxNr = 10;

    TMP_Text text;

    Coroutine countdownCorutine;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        countdownCorutine = StartCoroutine(Countdown());
    }

    private void OnDisable()
    {
        if(countdownCorutine != null)
        {
            StopCoroutine(countdownCorutine);
        }
    }

    IEnumerator Countdown()
    {
        for(int i = countdownMaxNr; i > 0; i--)
        {
            text.SetText(i.ToString());
            yield return new WaitForSecondsRealtime(1f);
        }

        if(gameObject.activeSelf)
        {
            graphicsScript.WarningWindowAnswer(false);
        }
    }


    
    
}
