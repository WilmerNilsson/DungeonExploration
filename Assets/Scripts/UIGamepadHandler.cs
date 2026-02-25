using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIGamepadHandler : MonoBehaviour
{
    private EventSystem eventSystem;
    private PlayerInput _playerInput;
    private string ControlScheme;
    public static UIGamepadHandler instance;
    private List<GameObject> buttons = new List<GameObject>();

    private void Awake()
    {
        instance = this;
        Debug.Log(instance);
        eventSystem = GetComponent<EventSystem>();
        _playerInput = FindAnyObjectByType<PlayerInput>();
    }

    public void OpenMenu(GameObject button)
    {
        buttons.Add(button);
        eventSystem.SetSelectedGameObject(button);
    }

    public void CloseMenu(GameObject button)
    {
        buttons.Remove(button);
        if (buttons.Count > 0 && eventSystem.currentSelectedGameObject == button)
        {
            eventSystem.SetSelectedGameObject(buttons[^1]);
        }
    }
    
    private void Update()
    {
        if (buttons.Count == 0)
        {
            return;
        }
        if (eventSystem.currentSelectedGameObject)
        {
            Debug.DrawLine(eventSystem.currentSelectedGameObject.transform.position, Vector3.zero);
        }

        if (_playerInput.currentControlScheme == "Controler" && !eventSystem.currentSelectedGameObject)
        {
            eventSystem.SetSelectedGameObject(buttons[^1]);
        }
    }
}
