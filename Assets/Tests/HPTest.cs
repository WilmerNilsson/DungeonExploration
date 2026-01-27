
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
    private Health health;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
        health = Object.FindAnyObjectByType<Health>();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        yield return SceneManager.UnloadSceneAsync(scene);
    }

    [UnityTest]
    public IEnumerator HPInitializeAndDamageTest()
    {
        yield return null;
        Assert.AreEqual(health.GetHealth(), health.GetMaxHealth(), "Max hp and currentHp are not equal", this);
        Assert.Less(1, health.GetHealth(), "health too low for testing");
        Assert.IsFalse(health.Dead, "spawned dead");
        int oldHP = health.GetHealth();
        health.TakeDamage(1);
        Assert.AreEqual(health.GetHealth()+1, oldHP, "health did not decrement by 1 after taking 1 damage");
        health.TakeDamage(health.GetHealth());
        Assert.AreEqual(health.GetHealth(), 0, "health did not reach 0 after taking a amount of damage equal to current health");
        Assert.IsTrue(health.Dead, "Dead bool is false after reaching 0 health");
    }
}
