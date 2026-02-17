using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static UnityEngine.Analytics.IAnalytic;

[TestFixture]
public class WorldSaveTest
{
    private Scene scene;
    private const string sceneName = "WorldSaveTestScene";

    [UnitySetUp]
    public IEnumerator Setup()
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        yield return SceneManager.UnloadSceneAsync(scene);
    }

    [UnityTest]
    public IEnumerator SaveDataCreationTest()
    {
        bool SecondGo = false;

        yield return null;
        SavefileData.WorldData worldData = WorldDataCreator.CreateWorldData();

        //some of theses are not compared to thier actual data and only trough helper,
        //but the idea is that if the helper is doing something wrong the parity would be lost on the second go

        REDO:

        #region Item pickups / dropped items
        List<DungeonSaveData.DroppedItem> droppedItems = worldData.DungeonSaveData.DroppedItems;

        ItemPickup[] itemPickups = GameObject.FindObjectsByType<ItemPickup>(FindObjectsSortMode.None);

        Assert.AreEqual(itemPickups.Length, droppedItems.Count, "save file dropped items amount is not matching the world", this);

        List<ItemPickup> unmatchedItemPickups = itemPickups.ToList();

        for (int i = 0; i < droppedItems.Count; i++)
        {
            for (int j = 0; j < unmatchedItemPickups.Count; j++)
            {
                bool match = ItemMatch(unmatchedItemPickups[j], droppedItems[i]);

                if (match)
                {
                    unmatchedItemPickups.RemoveAt(j);
                    break;
                }
            }
        }

        Assert.AreEqual(0, unmatchedItemPickups.Count, "not all item pickups got a match", this);
        #endregion

        #region Containers
        List<DungeonSaveData.Container> containers = worldData.DungeonSaveData.Containers;

        SaveFileHelperContainer[] containerHelpers = GameObject.FindObjectsByType<SaveFileHelperContainer>(FindObjectsSortMode.None);
        Assert.AreEqual(containerHelpers.Length, containers.Count, "helpers and containers are different lengths", this);

        List<SaveFileHelperContainer> unmatchedContainers = containerHelpers.ToList();

        for (int i = 0; i < containers.Count; i++)
        {
            for (int j = 0; j < unmatchedContainers.Count; j++)
            {
                bool match = ContainerMatch(unmatchedContainers[j], containers[i]);

                if (match)
                {
                    unmatchedContainers.RemoveAt(j);
                    break;
                }
            }
        }

        Assert.AreEqual(0, unmatchedContainers.Count, "not all Containers got a match", this);
        #endregion

        #region Enemies
        List<DungeonSaveData.Enemy> enemies = worldData.DungeonSaveData.Enemies;
        SaveFileHelperEnemy[] enemyHelpers = GameObject.FindObjectsByType<SaveFileHelperEnemy>(FindObjectsSortMode.None);
        Assert.AreEqual(enemyHelpers.Length, enemies.Count, "helpers and enemies are different lengths", this);

        List<SaveFileHelperEnemy> unmatchedEnemies = enemyHelpers.ToList();

        for (int i = 0; i < enemies.Count; i++)
        {
            for (int j = 0; j < unmatchedEnemies.Count; j++)
            {
                bool match = EnemyMatch(unmatchedEnemies[j], enemies[i]);

                if (match)
                {
                    unmatchedEnemies.RemoveAt(j);
                    break;
                }
            }
        }

        Assert.AreEqual(0, unmatchedEnemies.Count, "not all item enemies got a match", this);
        #endregion

        #region Player
        PlayerSaveData playerSaveData = GameObject.FindAnyObjectByType<SaveFileHelperPlayer>(FindObjectsInactive.Exclude).GetData();

        Assert.AreEqual(playerSaveData.Position, worldData.PlayerSaveData.Position, "player possistion are not equal", this);
        Assert.AreEqual(playerSaveData.Rotation, worldData.PlayerSaveData.Rotation, "player rotation are not equal", this);
        Assert.AreEqual(playerSaveData.CurrentHP, worldData.PlayerSaveData.CurrentHP, "player CurrentHP are not equal", this);
        Assert.AreEqual(playerSaveData.MaxHP, worldData.PlayerSaveData.MaxHP, "player MaxHP are not equal", this);
        Assert.AreEqual(playerSaveData.Sanity, worldData.PlayerSaveData.Sanity, "player Sanity are not equal", this);
        Assert.AreEqual(playerSaveData.Hunger, worldData.PlayerSaveData.Hunger, "player MaxHP are not equal", this);

        Assert.IsTrue(InventoryMatch(playerSaveData.Inventory, worldData.PlayerSaveData.Inventory));
        #endregion

        if(SecondGo == false)
        {
            SecondGo = true;

            GameObject.Destroy(GameObject.Find("DeleteObject"));
            
            yield return null;

            GameObject.FindFirstObjectByType<WorldFromDataCreator>().InitializeWorld(worldData);

            goto REDO;
        }


        bool ItemMatch(ItemPickup pickup, DungeonSaveData.DroppedItem droppedItem)
        {
            if (pickup.ItemID != droppedItem.ItemID)
            {
                return false;
            }

            if (pickup.transform.position != droppedItem.Position)
            {
                return false;
            }

            if (pickup.transform.rotation != droppedItem.Rotation)
            {
                return false;
            }

            return true;
        }

        bool EnemyMatch(SaveFileHelperEnemy helper, DungeonSaveData.Enemy enemy)
        {
            DungeonSaveData.Enemy data = helper.GetData();

            if (data.PrefabID != enemy.PrefabID) return false;
            if (data.Position != enemy.Position) return false;
            if (data.Rotation != enemy.Rotation) return false;
            if (data.CurrentHP != enemy.CurrentHP) return false;

            return true;
        }

        bool ContainerMatch(SaveFileHelperContainer saveFileHelperContainer, DungeonSaveData.Container container)
        {
            DungeonSaveData.Container data = saveFileHelperContainer.GetData();

            if(data.PrefabID != container.PrefabID) return false;
            if(data.Position != container.Position) return false;
            if(data.Rotation != container.Rotation) return false;
            if(!InventoryMatch(data.Inventory, container.Inventory)) return false;

            return true;
        }

        bool InventoryMatch(InventorySaveData a, InventorySaveData b)
        {
            if(a.Items.Count != b.Items.Count) return false;

            for(int i = 0; i < a.Items.Count; i++)
            {
                if(a.Items[i].Name != b.Items[i].Name) return false;
                if (a.Items[i].Slot != b.Items[i].Slot) return false;
            }

            return true;
        }
    }
}
