using UnityEngine;

public static class WorldDataCreator
{
    /// <summary>
    /// without settings
    /// </summary>
    public static void CreateWorldData()
    {
        DungeonSaveData dungeonSaveData = new();

        AddContainerData(dungeonSaveData);
        AddDroppedItemData(dungeonSaveData);
        AddEnemyData(dungeonSaveData);

        PlayerSaveData playerSaveData = GameObject.FindAnyObjectByType<SaveFileHelperPlayer>().GetData();

        SavefileData.WorldData worldData = new(playerSaveData, dungeonSaveData);

        static void AddEnemyData(DungeonSaveData dungeonSaveData)
        {
            SaveFileHelperEnemy[] enemies = GameObject.FindObjectsByType<SaveFileHelperEnemy>(FindObjectsSortMode.None);

            foreach (SaveFileHelperEnemy enemy in enemies)
            {
                dungeonSaveData.Enemies.Add(enemy.GetData());
            }
        }

        static void AddDroppedItemData(DungeonSaveData dungeonSaveData)
        {
            ItemPickup[] itemPickups = GameObject.FindObjectsByType<ItemPickup>(FindObjectsSortMode.None);

            foreach (ItemPickup pickup in itemPickups)
            {
                DungeonSaveData.DroppedItem data = new();

                data.Name = pickup.PrefabID;
                data.Pos = pickup.transform.position;

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
