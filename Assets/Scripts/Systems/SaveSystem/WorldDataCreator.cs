using UnityEngine;

public static class WorldDataCreator
{
    /// <summary>
    /// without settings
    /// </summary>
    public static void CreateWorldData(out DungeonSaveData dungeonSaveData, out PlayerSaveData playerSaveData)
    {
        dungeonSaveData = new();

        AddContainerData(dungeonSaveData);
        AddDroppedItemData(dungeonSaveData);
        AddEnemyData(dungeonSaveData);

        playerSaveData = GameObject.FindAnyObjectByType<SaveFileHelperPlayer>(FindObjectsInactive.Exclude).GetData();
        dungeonSaveData.Initialized = true;

        return;

        static void AddEnemyData(DungeonSaveData dungeonSaveData)
        {
            SaveFileHelperEnemy[] enemies = GameObject.FindObjectsByType<SaveFileHelperEnemy>(FindObjectsSortMode.None);

            foreach (SaveFileHelperEnemy enemy in enemies)
            {
                DungeonSaveData.Enemy? data = enemy.GetData();
                if (data != null) //it is null when enemy is dead;
                {
                    dungeonSaveData.Enemies.Add((DungeonSaveData.Enemy) data);
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
    }
}
