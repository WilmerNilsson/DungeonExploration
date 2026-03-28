using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimedUI : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private string newText;
    [SerializeField] private float newSize;
    [SerializeField] private string defaultText;
    [SerializeField] private float defaultSize;

    private IEnumerator DisplayForTime()
    {
        yield return new WaitForSeconds(time);
        textMeshProUGUI.text = defaultText;
        textMeshProUGUI.fontSize = defaultSize;
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        textMeshProUGUI.text = defaultText;
        textMeshProUGUI.fontSize = defaultSize;
    }

    public void Display()
    {
        textMeshProUGUI.text = newText;
        textMeshProUGUI.fontSize = newSize;
        StopAllCoroutines();
        StartCoroutine(DisplayForTime());
    }
}
