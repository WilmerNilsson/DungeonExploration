using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyIDLibrarySO", menuName = "Scriptable Objects/EnemyIDLibrarySO")]
public class EnemyLibrarySO : ScriptableObject
{
    [SerializeField] private Pair[] Enemies;
#nullable enable

    public bool TryGetPrefabByName(string name, [NotNullWhen(true)] out GameObject? prefab)
    {
        prefab = Enemies.FirstOrDefault(x => x.Name == name)?.Prefab;

        return prefab != null;
    }

    [System.Serializable]
    public class Pair
    {
        public string Name;
        public GameObject Prefab;
    }
}
