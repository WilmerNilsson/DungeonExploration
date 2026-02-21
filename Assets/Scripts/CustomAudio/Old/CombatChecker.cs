using System.Collections.Generic;
using CustomAudio;
using UnityEngine;

public static class CombatChecker
{
    private static List<GameObject> _enemiesChasing = new List<GameObject>();
    public static bool IsCombat {get; private set;}

    //Lägg till objekt i EnemiesChasing OM det inte redan finns i listan eller om de är null,
    //efter det checka combatState
    public static void AddToChaseList(GameObject gameObject) 
    {
        if (_enemiesChasing.Contains(gameObject) || gameObject == null) return;
        _enemiesChasing.Add(gameObject);
        CheckCombatState();
    }

    //Ta bort objekt från EnemiesChasing OM det inte är null och om det finns i listan,
    //efter det checka combatState
    public static void RemoveFromChaseList(GameObject gameObject)
    {
        if (gameObject == null) return;
        if (_enemiesChasing.Contains(gameObject)) _enemiesChasing.Remove(gameObject);
        CheckCombatState();
    }

    private static void CheckCombatState()
    {
        IsCombat = _enemiesChasing.Count > 0;
        AudioSystem.instance.ParameterHandler.SetGlobalParameter("Combat", _enemiesChasing.Count > 0 ? 1 : 0);
    }

    public static void RefreshCombatList()
    {
        for (int i = 0; i < _enemiesChasing.Count; i++)
        {
            if (!_enemiesChasing[i]) _enemiesChasing.Remove(_enemiesChasing[i]);
        }
        CheckCombatState();
    }

    public static void ResetCombatList()
    {
        _enemiesChasing = new List<GameObject>();
        CheckCombatState();
    }
}
