using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ContainerIDLibrarySO", menuName = "Scriptable Objects/ContainerIDLibrarySO")]
public class ContainerDLibrarySO : ScriptableObject
{
    [SerializeField] private Pair[] Containers;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public bool TryGetPrefabByName(string name, [NotNullWhen(true)] out GameObject? prefab)
    {
        prefab = Containers.First(x => x.Name == name).Prefab;

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
