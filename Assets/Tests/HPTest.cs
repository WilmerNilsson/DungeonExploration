using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class HPTest
{
    private Scene scene;
    private const string sceneName = "HealthAndDamageTestScene";
    //private Health health;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
        //health = Object.FindAnyObjectByType<Health>();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        yield return SceneManager.UnloadSceneAsync(scene);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void HPTestSimplePasses()
    {
        int maxHP = 1, currentHP = 1;
        Assert.AreEqual(maxHP, currentHP, "Max hp and currentHp are not equal", this);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator HPTestWithEnumeratorPasses()
    {
        int maxHP = 1, currentHP = 1;
        Assert.AreEqual(maxHP, currentHP, "Max hp and currentHp are not equal", this);
        yield return null;
        currentHP--;

    }
}
