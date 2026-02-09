using System.Collections.Generic;
using UnityEngine;

public static class CombatChecker
{
    private static List<GameObject> EnemiesChasing { get; set; } = new List<GameObject>();

    //Lägg till objekt i EnemiesChasing OM det inte redan finns i listan eller om de är null,
    //efter det checka combatState
    public static void AddToChaseList(GameObject gameObject) 
    {
        if (EnemiesChasing.Contains(gameObject) || gameObject == null) return;
        EnemiesChasing.Add(gameObject);
        CheckCombatState();
    }

    //Ta bort objekt från EnemiesChasing OM det inte är null och om det finns i listan,
    //efter det checka combatState
    public static void RemoveFromChaseList(GameObject gameObject)
    {
        if (gameObject == null) return;
        if (EnemiesChasing.Contains(gameObject)) EnemiesChasing.Remove(gameObject);
        CheckCombatState();
    }

    private static void CheckCombatState()
    {
        Debug.Log(EnemiesChasing.Count + " Enemies Chasing");
        if (AudioManager.IsValid)
        {
            AudioManager.Instance.SetGlobalParameter("Combat", EnemiesChasing.Count > 0 ? 1 : 0);
        }
    }
}
