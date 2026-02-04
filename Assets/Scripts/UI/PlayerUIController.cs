using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InvMaster invMaster;
    //we prob want a generall UI master later for a pause menu and such.

#if DEBUG
    private void OnValidate()
    {
        if (playerController == null) Debug.LogWarning("player controller is null", this);
    }
#endif

    public void OnInventory()
    {
        invMaster.ToggleInventory();
    }
}
