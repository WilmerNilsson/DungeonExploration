using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private GameObject child;
    private void Awake()
    {
        child.SetActive(false);
    }

    private void OnEnable()
    {
        GameObject.FindGameObjectWithTag("Player").TryGetComponent(out HumanoidInteract interact);
        interact.OnSee.AddListener(OnSee);
        interact.OnUnSee.AddListener(OnUnSee);
    }

    private void OnSee()
    {
        child.SetActive(true);
    }

    private void OnUnSee()
    {
        child.SetActive(false);
    }
}
