using System;
using UnityEngine;

public interface IUIController
{
    public event Action OnChangeScreenAction;

    void GoToScreen(GameObject newScreen);
    void ChangeCanUnpause(bool canUnpause);
    void TogglePauseMenu();
    void ChangeUseWarningScreen(bool value);
}
