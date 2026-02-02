
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class HPSOTest
{
    private Scene scene;
    private const string sceneName = "HealthSOAndDamageTestScene";
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
        Assert.AreEqual(health.CurrentHealth, health.MaxHealth, "Max hp and currentHp are not equal", this);
        Assert.Less(1, health.CurrentHealth, "health too low for testing");
        Assert.IsFalse(health.Dead, "spawned dead");
        int oldHP = health.CurrentHealth;

        health.TakeDamage(1);
        Assert.AreEqual(health.CurrentHealth+1, oldHP, "health did not decrement by 1 after taking 1 damage");
        health.TakeDamage(health.CurrentHealth);
        Assert.AreEqual(0, health.CurrentHealth, "health did not reach 0 after taking a amount of damage equal to current health");
        Assert.IsTrue(health.Dead, "Dead bool is false after reaching 0 health");
    }

    [UnityTest]
    public IEnumerator HPAndDamageInterractTest()
    {
        yield return null;
        Assert.AreEqual(health.CurrentHealth, health.MaxHealth, "Max hp and currentHp are not equal", this);
        Assert.Less(1, health.CurrentHealth, "health too low for testing");
        Assert.IsFalse(health.Dead, "spawned dead");

        int oldHP = health.CurrentHealth;

        SimpleDamage simpleDamage = Object.FindAnyObjectByType<SimpleDamage>();
        simpleDamage.transform.position = health.transform.position;
        yield return null;

        Assert.Less(health.CurrentHealth, oldHP, "health did not decrement after damage collider was placed over it");
    }
}
