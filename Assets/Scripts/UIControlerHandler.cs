using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class UIControlerHandler : MonoBehaviour
{
    [SerializeField] private GameObject FirstSelected;
    private EventSystem _eventSystem;
    private PlayerInput _playerInput;
    private string ControlScheme;

    private void OnEnable()
    {
        _eventSystem = FindAnyObjectByType<EventSystem>();
        _playerInput = FindAnyObjectByType<PlayerInput>();
        ControlScheme = _playerInput.currentControlScheme;
        _eventSystem.SetSelectedGameObject(FirstSelected);
    }

    private void Update()
    {
        if (_eventSystem.currentSelectedGameObject)
        {
            Debug.DrawLine(_eventSystem.currentSelectedGameObject.transform.position, Vector3.zero);
        }
        if (_playerInput.currentControlScheme != ControlScheme)
        {
            ControlScheme =  _playerInput.currentControlScheme;
            if (ControlScheme == "Controler")
            {
                _eventSystem.SetSelectedGameObject(FirstSelected);
            }
        }
    }
}
