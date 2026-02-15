using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyIDLibrarySO", menuName = "Scriptable Objects/EnemyIDLibrarySO")]
public class EnemyIDLibrarySO : ScriptableObject
{
    [SerializeField] private Pair[] Enemies;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public bool TryGetPrefabByName(string name, [NotNullWhen(true)] out GameObject? prefab)
    {
        prefab = Enemies.First(x => x.Name == name).Prefab;

        return prefab != null;
    }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    [System.Serializable]
    public class Pair
    {
        public string Name;
        public GameObject Prefab;
    }
}
