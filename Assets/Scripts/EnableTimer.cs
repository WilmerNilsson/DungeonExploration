using System.Collections;
using UnityEngine;

public class EnableTimer : MonoBehaviour
{
    [SerializeField] private GameObject toggleObject;
    [SerializeField] private float timer;
    void Start()
    {
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSecondsRealtime(timer);

        toggleObject.SetActive(true);
    }
}
