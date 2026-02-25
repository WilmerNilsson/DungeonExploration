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
        eventSystem = GetComponent<EventSystem>();
        _playerInput = FindAnyObjectByType<PlayerInput>();
        ControlScheme = _playerInput.currentControlScheme;
        instance = this;
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
        if (_playerInput.currentControlScheme != ControlScheme)
        {
            ControlScheme =  _playerInput.currentControlScheme;
            if (ControlScheme == "Controler")
            {
                eventSystem.SetSelectedGameObject(buttons[^1]);
            }
        }
    }
}
