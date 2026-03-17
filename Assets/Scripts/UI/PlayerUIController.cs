using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (GameManagerSO.DevConsoleActive) return;

        if (context.canceled && InvMasterBase.Instance is InvMaster invMaster)
            invMaster.ToggleInventory();
    }

    public void OnMinimap(InputAction.CallbackContext context)
    {
        if (GameManagerSO.DevConsoleActive) return;

        if (context.canceled) MinimapMaster.Instance.ToggleMinimap();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (GameManagerSO.DevConsoleActive) return;

        if (context.canceled) GameObject.FindGameObjectWithTag("MainUI").GetComponent<InGameUIController>().TogglePauseMenu();
    }

    public void OnDevConsole(InputAction.CallbackContext context)
    {
        if (context.canceled) DevConsoleGha.Instance.ToggeDevConsole();
    }
}
