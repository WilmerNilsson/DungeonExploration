using System.Collections.Generic;
using UnityEngine;

public class TownFromDataCreator : MonoBehaviour
{
    [SerializeField] private PlayerCashSO playerCashSO;
    [SerializeField] private ItemLibrarySO itemLibrary;
    [SerializeField] private BlacksmithUI blacksmithUI;
    [SerializeField] private List<DialogueContainer> dialogueContainers;
    public int RunCount;
#nullable enable

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (playerCashSO == null) Debug.LogError("player cash is null", this);
        if (itemLibrary == null) Debug.Log("item library is null", this);
        if (blacksmithUI == null) Debug.Log("BlacksmitUI is null", this);
        if (dialogueContainers.Count == 0) Debug.Log("No dialogue containers", this);
    }
#endif

    private void Start()
    {
        if(GameManagerSO.Instance.TryConsumeSavefileData(out SavefileData? data))
        {
            //if world is null let it throw error

            playerCashSO.SetCash(data.PlayerGold);
            blacksmithUI.GiveSaveData(data.DonatedWeapons);
            foreach (var dialogueContainer in data.DialogueContainers)
            {
                dialogueContainer.SetDialogueData(data.DialogueContainers.Find(x => x.name == dialogueContainer.name));
            }
            RunCount = data.PlayerSaveData.RunCount;

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

    public List<string> GetDonatedWeapons()
    {
        return blacksmithUI.GetSaveData();
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

    public List<DialogueContainer> GetDialogueContainers()
    {
        return dialogueContainers;
    }
}
