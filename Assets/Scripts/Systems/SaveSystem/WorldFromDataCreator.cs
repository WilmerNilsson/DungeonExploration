using UnityEngine;

#nullable enable

public class WorldFromDataCreator : MonoBehaviour
{
    [SerializeField] private ItemLibrarySO itemLibrary;
    [SerializeField] private ContainerLibrarySO containerLibrary;
    [SerializeField] private EnemyLibrarySO enemyLibrary;

#if DEBUG
    private void OnValidate()
    {
        if (itemLibrary == null) Debug.LogWarning("item library is null", this);
        if (containerLibrary == null) Debug.LogWarning("container library is null", this);
        if (enemyLibrary == null) Debug.LogWarning("enemy library is null", this);
    }
#endif

    private void Awake()
    {
        if(GameManagerSO.Instance.TryConsumeSavefileData(out SavefileData? data))
        {
            if(data.World != null)
            {
                InitializeWorld(data.World);
            }
            else
            {
                CreateNewWorld();
            }
        }
#if DEBUG && !UNITY_INCLUDE_TESTS
        else
        {
            Debug.LogError("World from data creator tried to consume save file data, but it failed", this);
        }
#endif
    }

    private void CreateNewWorld()
    {

    }

#if UNITY_INCLUDE_TESTS
    public void InitializeWorld(SavefileData.WorldData worldData)
#else
    private void InitializeWorld(SavefileData.WorldData worldData)
#endif
    {
        InitializeContainers(worldData);
        InitializeEnemies(worldData);
        InitializeDroppedItems(worldData);

        InitializePlayer(worldData);

        void InitializePlayer(SavefileData.WorldData worldData)
        {
            SaveFileHelperPlayer helper = FindFirstObjectByType<SaveFileHelperPlayer>();

#if DEBUG
            if(helper == null)
            {
                Debug.LogError("could not find player save file helper in scene", this);
                return;
            }
#endif

            helper.Initialize(worldData.PlayerSaveData);
        }

        void InitializeDroppedItems(SavefileData.WorldData worldData)
        {
            foreach(DungeonSaveData.DroppedItem item in worldData.DungeonSaveData.DroppedItems)
            {
                if(itemLibrary.TryGetItemPairByName(item.ItemID, out ItemPairing? pair))
                {
                    Instantiate(pair.WorldPrefab, item.Position, item.Rotation);
                }
#if DEBUG
                else
                {
                    Debug.LogError("could not get item pair from save file ID: " + item.ItemID, this);
                    continue;
                }
#endif
            }
        }

        void InitializeEnemies(SavefileData.WorldData worldData)
        {
            foreach (DungeonSaveData.Enemy enemy in worldData.DungeonSaveData.Enemies)
            {
                if (enemyLibrary.TryGetPrefabByName(enemy.PrefabID, out GameObject? prefab))
                {
                    GameObject containerInstance = Instantiate(prefab);

                    if (containerInstance.TryGetComponent<SaveFileHelperEnemy>(out SaveFileHelperEnemy helper))
                    {
                        helper.Intialize(enemy);
                    }
#if DEBUG
                    else
                    {
                        Debug.LogError("instanciated enemy does not have helper, ID: " + enemy.PrefabID, this);
                        continue;
                    }
#endif
                }
#if DEBUG
                else
                {
                    Debug.LogError("could not get enemy prefab from save file world data, ID: " + enemy.PrefabID, this);
                    continue;
                }
#endif
            }
        }

        void InitializeContainers(SavefileData.WorldData worldData)
        {
            foreach (DungeonSaveData.Container container in worldData.DungeonSaveData.Containers)
            {
                if (containerLibrary.TryGetPrefabByName(container.PrefabID, out GameObject? prefab))
                {
                    GameObject containerInstance = Instantiate(prefab);

                    if (containerInstance.TryGetComponent<SaveFileHelperContainer>(out SaveFileHelperContainer helper))
                    {
                        helper.Intialize(container);
                    }
#if DEBUG
                    else
                    {
                        Debug.LogError("instanciated container does not have helper, ID: " + container.PrefabID, this);
                        continue;
                    }
#endif
                }
#if DEBUG
                else
                {
                    Debug.LogError("could not get container prefab from save file world data, ID: " + container.PrefabID, this);
                    continue;
                }
#endif
            }
        }
    }
}
