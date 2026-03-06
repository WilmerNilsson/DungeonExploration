using UnityEngine;

public static class TownDataCreator
{
    public class TownData
    {
        public int Cash;
        public InventorySaveData Inventory;
        public InventorySaveData Equipment;
    }

    public static TownData GetTownData()
    {
        TownData data = new();

        //will not null check since we want it to throw errors if needed

        SaveFileHelperTown helper = GameObject.FindAnyObjectByType<SaveFileHelperTown>();

        data.Cash = helper.GetCash();
        data.Inventory = helper.GetPlayerInventory();
        data.Equipment = helper.GetPlayerEquipment();

        return data;
    }
}
