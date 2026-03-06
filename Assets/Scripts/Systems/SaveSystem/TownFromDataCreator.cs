using UnityEngine;

public class TownFromDataCreator : MonoBehaviour
{
    [SerializeField] private PlayerCashSO playerCashSO;
    [SerializeField] private ItemLibrarySO itemLibrary;
#nullable enable

#if UNITY_EDITOR
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

            if(data.PlayerSaveData != null)
            {
                SaveFileHelperContainer.PopulateInventory(itemLibrary, data.PlayerSaveData.Inventory, InvMasterBase.Instance.PlayerInventory);
                SaveFileHelperContainer.PopulateInventory(itemLibrary, data.PlayerSaveData.Equipment, InvMasterBase.Instance.EquipmentGrid);
            }
            else
            {
                Debug.LogWarning("player save data is null", this);
            }
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
    public InventorySaveData GetPlayerEquipment()
    {
        return new(InvMasterBase.Instance.EquipmentGrid.GetInventoryData());
    }
}
