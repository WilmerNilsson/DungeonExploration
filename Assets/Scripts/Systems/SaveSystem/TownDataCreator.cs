using UnityEngine;
using System.Collections.Generic;

public static class TownDataCreator
{
    public class TownData
    {
        public int Cash;
        public InventorySaveData Inventory;
        public InventorySaveData Equipment;
        public List<string> DonatedWeapons;
        public List<DialogueContainer>  DialogueContainers;
    }

    public static TownData GetTownData()
    {
        TownData data = new();

        //will not null check since we want it to throw errors if needed

        TownFromDataCreator helper = GameObject.FindAnyObjectByType<TownFromDataCreator>();

        data.Cash = helper.GetCash();
        data.Inventory = helper.GetPlayerInventory();
        data.Equipment = helper.GetPlayerEquipment();
        data.DonatedWeapons = helper.GetDonatedWeapons();
        data.DialogueContainers = helper.GetDialogueContainers();

        return data;
    }
}
