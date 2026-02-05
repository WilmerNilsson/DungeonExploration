using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreenController : MonoBehaviour
{
    private GameObject toggleObject;
    private GameObject playerObject;

    private void OnEnable()
    {
        toggleObject = transform.GetChild(0).gameObject;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerObject.GetComponentInChildren<Health>().OnDeathAction += ShowDeathScreen;
    }

    private void ShowDeathScreen()
    {
        toggleObject.SetActive(true);
    }

    public void HideDeathScreen()
    {
        toggleObject.SetActive(false);
    }

    public void ActivatePlayer()
    {
        playerObject.SetActive(true);
    }
}
