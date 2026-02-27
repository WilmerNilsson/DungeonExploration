using UnityEngine;

public class SaveFileHelperTown : MonoBehaviour
{
    [SerializeField] private PlayerCashSO playerCashSO;
    [SerializeField] private ItemLibrarySO itemLibrary;
#nullable enable

#if DEBUG
    private void OnValidate()
    {
        if (playerCashSO == null) Debug.LogError("player cash is null", this);
        if (itemLibrary == null) Debug.Log("item library is null", this);
    }
#endif

    private void Start()
    {
        if(GameManagerSO.Instance.TryConsumeSavefileData(out SavefileData? data))
        {
            //if world is null let it throw error

            playerCashSO.SetCash(data.PlayerGold);

            SaveFileHelperContainer.PopulateInventory(itemLibrary, data.World!.PlayerSaveData.Inventory, InvMasterBase.Instance.PlayerInventory);
        }
        else
        {
            Debug.LogWarning("could not consume save file", this);
        }
    }

    public int GetCash()
    {
        return playerCashSO.CurrentCash;
    }

    public InventorySaveData GetPlayerInventory()
    {
        return new(InvMasterBase.Instance.PlayerInventory.GetInventoryData());
    }
}
