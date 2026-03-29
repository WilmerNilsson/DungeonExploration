using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WorldFromDataCreator : MonoBehaviour
{
    [SerializeField] private ItemLibrarySO itemLibrary;
    [SerializeField] private ContainerLibrarySO containerLibrary;
    [SerializeField] private EnemyLibrarySO enemyLibrary;
    [SerializeField] private List<GameObject> newWorldObjects;
    [SerializeField] private List<SaveFileHelperEnemy> newWorldEnemies;
    [SerializeField] public List<DialogueContainer> dialogueContainers;
#nullable enable

#if UNITY_EDITOR
    [SerializeField] private bool reimportContainerAndItemsInNewWorld;
    [SerializeField] private bool importEnemiesAndGiveID;
    private void OnValidate()
    {
        if (itemLibrary == null) Debug.LogWarning("item library is null", this);
        if (containerLibrary == null) Debug.LogWarning("container library is null", this);
        if (enemyLibrary == null) Debug.LogWarning("enemy library is null", this);

        if(importEnemiesAndGiveID)
        {
            importEnemiesAndGiveID = false;

            newWorldEnemies = new();

            int currentID = 0;

            foreach (var enemy in GameObject.FindObjectsByType<SaveFileHelperEnemy>(FindObjectsSortMode.None))
            {
                newWorldEnemies.Add(enemy);
                enemy.SetID(currentID);
                currentID++;

                EditorUtility.SetDirty(enemy);
            }
        }

        if(reimportContainerAndItemsInNewWorld)
        {
            reimportContainerAndItemsInNewWorld = false;

            newWorldObjects = new List<GameObject>();
            foreach (var container in GameObject.FindObjectsByType<SaveFileHelperContainer>(FindObjectsSortMode.None))
            {
                newWorldObjects.Add(container.gameObject);
            }

            foreach (var item in GameObject.FindObjectsByType<ItemPickup>(FindObjectsSortMode.None))
            {
                newWorldObjects.Add(item.gameObject);
            }
        }
    }
#endif

    private void Start()
    {
        if (GameManagerSO.Instance.SavefileManager.TryConsumeSavefileData(out SavefileData? data))
        {
            if (data.DialogueSaves.Count > 0)
            {
                for (int i = 0; i < data.DialogueSaves.Count; i++)
                {
                    dialogueContainers.Find(x => x.name == data.DialogueSaves[i].TreeName).SetDialogueData(data.DialogueSaves[i]);
                }
            }

            if(data.Dungeon != null && data.Dungeon.Initialized)
            {
                MinimapMaster minimapMaster = FindFirstObjectByType<MinimapMaster>();
                Debug.Log("setting minimap master");
                minimapMaster.GetSO().SetToData(data.Dungeon.minimapComponentData);

                DestoryNewWorld(data.Dungeon);
                InitializeWorld(data.Dungeon, data.PlayerSaveData);
            }
            else
            {
                CreateNewWorld();
            }
        }
#if!UNIT_TESTS
        else
        {
            Debug.LogError("World from data creator tried to consume save file data, but it failed", this);
        }
#endif
    }

    //TODO remake initialize system to not use inactive objects
    private void DestoryNewWorld(DungeonSaveData dungeonSaveData)
    {
        Debug.Log("destroying new world items");

        foreach (var item in newWorldObjects)
        {
            Destroy(item);
        }

        //big O is crying, let it. (TODO calm big O) (n^2 club)
        //prob go trough order under -enemies- for the bigO(n) feel good experience
        foreach (var enemy in newWorldEnemies)
        {
            bool isDead = true;

            foreach(var data in dungeonSaveData.Enemies)
            {
                if(data.UniqueID == enemy.UniqueID) //if there is a clone of the enemy in new objects
                {
                    isDead = false;
                    Destroy(enemy.gameObject);
                    break;
                }
            }

            if (isDead && !enemy.ShouldRespawn())
            {
                Destroy(enemy.gameObject);
            }
        }
    }

    private void CreateNewWorld()
    {
        Debug.Log("creating new world");

        foreach (var item in newWorldObjects)
        {
            item.gameObject.SetActive(true);
        }

        SaveFileHelperPlayer helper = FindFirstObjectByType<SaveFileHelperPlayer>();
        helper.InitializeNew();
    }

#if UNIT_TESTS
    public void InitializeWorld(DungeonSaveData dungeonSaveData, PlayerSaveData playerSaveData)
#else
    private void InitializeWorld(DungeonSaveData? dungeonSaveData, PlayerSaveData? playerSaveData)
#endif
    {
        if (dungeonSaveData != null)
        {
            InitializeContainers(dungeonSaveData);
            InitializeEnemies(dungeonSaveData);
            InitializeDroppedItems(dungeonSaveData);
            InitializeEnabledObjects(dungeonSaveData);
        }


        InitializePlayer(playerSaveData);

        void InitializeEnabledObjects(DungeonSaveData data)
        {
            EnabledHelperCompanion[] enabledHelpers = GameObject.FindObjectsByType<EnabledHelperCompanion>(FindObjectsSortMode.None);

            foreach (EnabledHelperCompanion helper in enabledHelpers)
            {
                if (data.EnabledObjects.Contains(helper.UniqueID))
                {
                    helper.EnableFromSave();
                }
            }
        }

        void InitializePlayer(PlayerSaveData? playerSaveData)
        {
            if(playerSaveData == null)
            {
                Debug.Log("player data null, skipping initialize in helper", this);
                return;
            }

            SaveFileHelperPlayer helper = FindFirstObjectByType<SaveFileHelperPlayer>();

            if(helper == null)
            {
                Debug.LogError("could not find player save file helper in scene", this);
                return;
            }

            helper.Initialize(playerSaveData);
        }

        void InitializeDroppedItems(DungeonSaveData dungeonSaveData)
        {
            if (dungeonSaveData.DroppedItems == null)
            {
                Debug.Log("dropped items null, skipping spawning", this);
                return;
            }

            foreach (DungeonSaveData.DroppedItem item in dungeonSaveData.DroppedItems)
            {
                if(itemLibrary.TryGetItemPairByName(item.ItemID, out ItemPairing? pair))
                {
                    GameObject newItem = Instantiate(pair.WorldPrefab, item.Position, item.Rotation);

                    if (newItem.TryGetComponent(out IExtraDataHelper helper))
                    {
                        helper.GiveExtraData(item.ExtraJsonSerializeData);
                    }
                }
                else
                {
                    Debug.LogError("could not get item pair from save file ID: " + item.ItemID, this);
                    continue;
                }
            }
        }

        void InitializeEnemies(DungeonSaveData dungeonSaveData)
        {
            if(dungeonSaveData.Enemies == null)
            {
                Debug.Log("enemies save data null, skipping initialize");
                return;
            }
            foreach (DungeonSaveData.Enemy enemy in dungeonSaveData.Enemies)
            {
                if (enemyLibrary.TryGetPrefabByName(enemy.PrefabID, out GameObject? prefab))
                {
                    GameObject containerInstance = Instantiate(prefab);

                    if (containerInstance.TryGetComponent<SaveFileHelperEnemy>(out SaveFileHelperEnemy helper))
                    {
                        helper.Intialize(enemy);
                    }
                    else
                    {
                        Debug.LogError("instanciated enemy does not have helper, ID: " + enemy.PrefabID, this);
                        continue;
                    }
                }
                else
                {
                    Debug.LogError("could not get enemy prefab from save file world data, ID: " + enemy.PrefabID, this);
                    continue;
                }
            }
        }

        void InitializeContainers(DungeonSaveData dungeonSaveData)
        {
            if(dungeonSaveData.Containers == null)
            {
                Debug.Log("container save data null, skipping initialize", this);
                return;
            }
            foreach (DungeonSaveData.Container container in dungeonSaveData.Containers)
            {
                if (containerLibrary.TryGetPrefabByName(container.PrefabID, out GameObject? prefab))
                {
                    GameObject containerInstance = Instantiate(prefab);
                    if (containerInstance.TryGetComponent<SaveFileHelperContainer>(out SaveFileHelperContainer helper))
                    {
                        helper.Intialize(container);
                    }
                    else
                    {
                        Debug.LogError("instanciated container does not have helper, ID: " + container.PrefabID, this);
                        continue;
                    }
                }
                else
                {
                    Debug.LogError("could not get container prefab from save file world data, ID: " + container.PrefabID, this);
                    continue;
                }
            }
        }
    }
}
