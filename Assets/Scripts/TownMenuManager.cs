using UnityEngine;
using UnityEngine.InputSystem;

public class TownMenuManager : MonoBehaviour
{
    [SerializeField] private bool isPaused = false;
    [SerializeField] private GameObject pauseMenuUI;

    public void TogglePause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isPaused = pauseMenuUI.activeSelf;
            isPaused = !isPaused;
            pauseMenuUI.SetActive(isPaused);
        }
    }
}
