using UnityEngine;

public static class WorldDataCreator
{
    /// <summary>
    /// without settings
    /// </summary>
    public static void CreateWorldData()
    {
        SavefileData.WorldData worldData = new();

        AddContainerData(worldData);
        AddDroppedItemData(worldData);
        AddEnemyData(worldData);

        static void AddEnemyData(SavefileData.WorldData worldData)
        {
            SaveFileHelperEnemy[] enemies = GameObject.FindObjectsByType<SaveFileHelperEnemy>(FindObjectsSortMode.None);

            foreach (SaveFileHelperEnemy enemy in enemies)
            {
                worldData.DungeonSaveData.Enemies.Add(enemy.GetData());
            }
        }

        static void AddDroppedItemData(SavefileData.WorldData worldData)
        {
            ItemPickup[] itemPickups = GameObject.FindObjectsByType<ItemPickup>(FindObjectsSortMode.None);

            foreach (ItemPickup pickup in itemPickups)
            {
                DungeonSaveData.DroppedItem data = new();

                data.Name = pickup.PrefabID;
                data.Pos = pickup.transform.position;

                worldData.DungeonSaveData.DroppedItems.Add(data);
            }
        }

        static void AddContainerData(SavefileData.WorldData worldData)
        {
            //we may just subscribe them to some sort of list when they spawn instead of doing this, since it can be slow
            SaveFileHelperContainer[] saveFileHelperContainers = GameObject.FindObjectsByType<SaveFileHelperContainer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (SaveFileHelperContainer helper in saveFileHelperContainers)
            {
                worldData.DungeonSaveData.Containers.Add(helper.GetData());
            }
        }
    }
}
