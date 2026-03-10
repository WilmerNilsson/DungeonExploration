using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootPoolSO", menuName = "Scriptable Objects/LootPoolSO")]
public class LootPoolSO : ScriptableObject
{
    public List<LootPool> lootPools;
}
