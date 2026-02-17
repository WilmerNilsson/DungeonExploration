
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

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
        yield return null;
        SavefileData.WorldData worldData = WorldDataCreator.CreateWorldData();

        List<DungeonSaveData.DroppedItem> droppedItems = worldData.DungeonSaveData.DroppedItems;

        ItemPickup[] itemPickups = GameObject.FindObjectsByType<ItemPickup>(FindObjectsSortMode.None);

        Assert.AreEqual(2, droppedItems.Count, "there are not 2 dropped items in the scene", this);

        

        Assert.AreEqual("HealthPotion", droppedItems[0].ID, "dropped item 0 is not HealthPotion ID", this);
        Assert.AreEqual("HealthPotion", droppedItems[1].ID, "dropped item 1 is not HealthPotion ID", this);

        

    }
}
