using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class WarningCountdown : MonoBehaviour
{
    [SerializeField] private GraphicsSettingsMaster graphicsScript;
    [SerializeField, Min(1)] private int countdownMaxNr = 10;

    private TMP_Text text;

    private Coroutine countdownCorutine;

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
