using UnityEngine;
using System.Collections.Generic;

public static class SaveDataCreator
{
    public class TownData
    {
        public int Cash;
        public InventorySaveData Inventory;
        public InventorySaveData Equipment;
        public List<string> DonatedWeapons;
        public List<DialogueSaveData> DialogueSaveDatas;
    }

    public static void CreateWorldData(out DungeonSaveData dungeonSaveData,
        out PlayerSaveData playerSaveData, out List<DialogueSaveData> dialogueSaveDatas)
    {
        dungeonSaveData = new();

        AddContainerData(dungeonSaveData);
        AddDroppedItemData(dungeonSaveData);
        AddEnemyData(dungeonSaveData);
        AddEnabledData(dungeonSaveData);

        dialogueSaveDatas = new();
        AddDialogueData(dialogueSaveDatas);

        playerSaveData = GameObject.FindAnyObjectByType<SaveFileHelperPlayer>(FindObjectsInactive.Exclude).GetData();
        dungeonSaveData.Initialized = true;

        return;

        static void AddEnabledData(DungeonSaveData dungeonSaveData)
        {
            EnabledHelperCompanion[] enabledHelpers = GameObject.FindObjectsByType<EnabledHelperCompanion>(FindObjectsSortMode.InstanceID);

            foreach(EnabledHelperCompanion helper in enabledHelpers)
            {
                dungeonSaveData.EnabledObjects.Add(helper.IsEnabledForSave());
            }
        }

        static void AddEnemyData(DungeonSaveData dungeonSaveData)
        {
            SaveFileHelperEnemy[] enemies = GameObject.FindObjectsByType<SaveFileHelperEnemy>(FindObjectsSortMode.None);

            foreach (SaveFileHelperEnemy enemy in enemies)
            {
                DungeonSaveData.Enemy? data = enemy.GetData();
                if (data != null) //it is null when enemy is dead;
                {
                    dungeonSaveData.Enemies.Add((DungeonSaveData.Enemy)data);
                }
            }
        }

        static void AddDroppedItemData(DungeonSaveData dungeonSaveData)
        {
            ItemPickup[] itemPickups = GameObject.FindObjectsByType<ItemPickup>(FindObjectsSortMode.None);

            foreach (ItemPickup pickup in itemPickups)
            {
                DungeonSaveData.DroppedItem data = new();

                data.ItemID = pickup.ItemID;
                data.Rotation = pickup.transform.rotation;
                data.Position = pickup.transform.position;

                dungeonSaveData.DroppedItems.Add(data);
            }
        }

        static void AddContainerData(DungeonSaveData dungeonSaveData)
        {
            //we may just subscribe them to some sort of list when they spawn instead of doing this, since it can be slow
            SaveFileHelperContainer[] saveFileHelperContainers = GameObject.FindObjectsByType<SaveFileHelperContainer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (SaveFileHelperContainer helper in saveFileHelperContainers)
            {
                dungeonSaveData.Containers.Add(helper.GetData());
            }
        }

        static void AddDialogueData(List<DialogueSaveData> dialogueSaveData)
        {
            WorldFromDataCreator worldFromDataCreator = GameObject.FindAnyObjectByType<WorldFromDataCreator>();
            if (worldFromDataCreator == null)
            {
                Debug.Log("No worldFromDataCreator found");
                return;
            }

            for (int i = 0; i < worldFromDataCreator.dialogueContainers.Count; i++)
            {
                dialogueSaveData.Add(new DialogueSaveData(worldFromDataCreator.dialogueContainers[i]));
            }
        }
    }

    public static TownData GetTownData()
    {
        TownData data = new();

        //will not null check since we want it to throw errors if needed

        TownSaveSystemInterface helper = GameObject.FindAnyObjectByType<TownSaveSystemInterface>();

        data.Cash = helper.GetCash();
        data.Inventory = helper.GetPlayerInventory();
        data.Equipment = helper.GetPlayerEquipment();
        data.DonatedWeapons = helper.GetDonatedWeapons();
        data.DialogueSaveDatas = helper.GetDialogueSaveDatas();

        return data;
    }
}
