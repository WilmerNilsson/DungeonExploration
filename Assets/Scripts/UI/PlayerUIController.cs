using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public void OnInventory()
    {
        InvMaster.Instance.ToggleInventory();
    }

    public void OnMinimap()
    {
        MinimapMaster.Instance.ToggleMinimap();
    }

    public void OnPause()
    {
        GameObject.FindGameObjectWithTag("MainUI").GetComponent<InGameUIController>().TogglePauseMenu();
    }

    public void OnDevConsole()
    {
        DevConsoleGha.Instance.ToggeDevConsole();
    }
}
