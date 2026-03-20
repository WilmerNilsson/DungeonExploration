using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class UIGamepadButtonSetter : MonoBehaviour
{
    [SerializeField] private GameObject FirstSelected;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (FirstSelected == null) Debug.LogWarning("first selected is null", this);
    }
#endif

    private void OnEnable()
    {
        UIGamepadHandler.Instance.OpenMenu(FirstSelected);
    }

    private void OnDisable()
    {
        UIGamepadHandler.Instance.CloseMenu(FirstSelected);
    }
}
