using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class UIGamepadButtonSetter : MonoBehaviour
{
    [SerializeField] private GameObject FirstSelected;

    private void OnEnable()
    {
        UIGamepadHandler.instance.OpenMenu(FirstSelected);
    }

    private void OnDisable()
    {
        UIGamepadHandler.instance.CloseMenu(FirstSelected);
    }
}
