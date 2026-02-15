using UnityEngine;

public static class WorldDataCreator
{
    /// <summary>
    /// without settings
    /// </summary>
    public static void CreateSaveFile()
    {
        SavefileData.WorldData worldData = new();

        //we may just subscribe them to some sort of list when they spawn instead of doing this, since it can be slow
        SaveFileHelperContainer[] saveFileHelperContainers = GameObject.FindObjectsByType<SaveFileHelperContainer>(FindObjectsInactive.Include,FindObjectsSortMode.None);

        foreach(SaveFileHelperContainer helper in saveFileHelperContainers)
        {
            worldData.DungeonSaveData.containers.Add(helper.GetData());
        }

        
    }
}
