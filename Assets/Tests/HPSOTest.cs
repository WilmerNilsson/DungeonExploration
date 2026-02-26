
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
    public IEnumerator HPSetTest()
    {
        yield return null;
        Assert.AreEqual(health.CurrentHealth, health.MaxHealth, "Max hp and currentHp are not equal", this);
        Assert.IsFalse(health.Dead, "Spawned dead");

        int oldHP = health.MaxHealth;
        health.SetMaxHealth(oldHP + 5);
        Assert.AreEqual(oldHP + 5, health.MaxHealth, "Max hp did not increase correctly", this);

        oldHP = health.MaxHealth;
        health.SetMaxHealth(oldHP - 3);
        Assert.AreEqual(oldHP - 3, health.MaxHealth, "Max hp did not increase correctly", this);

        health.SetMaxHealth(20);
        Assert.AreEqual(20, health.MaxHealth, "Max hp did not set to 20", this);

        health.SetCurrentHealth(20);
        Assert.AreEqual(20, health.CurrentHealth, "Current health did not set to 20", this);
        Assert.AreEqual(health.MaxHealth, health.CurrentHealth, "both health values not set to 20", this);

        health.SetCurrentHealth(15);
        Assert.AreEqual(15, health.CurrentHealth, "Current health did not set (decrease) to 15", this);

        health.SetCurrentHealth(17);
        Assert.AreEqual(17, health.CurrentHealth, "Current health did not set (increase) to 17", this);

        health.SetCurrentHealth(21);
        Assert.AreEqual(health.MaxHealth, health.CurrentHealth, "both health above max not set to max", this);

        health.SetCurrentHealth(0);
        Assert.AreEqual(0, health.CurrentHealth, "healt not set to 0", this);
        Assert.IsTrue(health.Dead, "health did not set dead on set 0", this);
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
        yield return new WaitForFixedUpdate();

        Assert.Less(health.CurrentHealth, oldHP, "health did not decrement after damage collider was placed over it");
    }
}
