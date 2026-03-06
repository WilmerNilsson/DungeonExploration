using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemMasterSO", menuName = "Scriptable Objects/ItemMasterSO")]
public class ItemLibrarySO : ScriptableObject
{
    [SerializeField] private ItemPairing[] itemPairs = { };

#if DEBUG && UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < itemPairs.Length; i++)
        {
            ItemPairing pair = itemPairs[i];
            if (pair.Name == null || pair.Name == string.Empty)
            {
                Debug.LogWarning($"pair index {i} lacks name", this);
            }
            if (pair.UIPrefab == null)
            {
                Debug.LogWarning($"pair index {i}, name {pair.Name} lacks UI prefab", this);
            }
            else if(!pair.UIPrefab.TryGetComponent<SimpleItem>(out _))
            {
                Debug.LogWarning($"pair index {i}, name {pair.Name} UI prefab has no SimpleItem Script", this);
            }

            if (pair.WorldPrefab == null)
            {
                Debug.LogWarning($"pair index {i}, name {pair.Name} lacks World prefab", this);
            }
            else if (!pair.WorldPrefab.TryGetComponent<ItemPickup>(out _))
            {
                Debug.LogWarning($"pair index {i}, name {pair.Name} World prefab has no ItemPickup Script", this);
            }
        }
    }
#endif

#nullable enable
    public bool TryGetItemPairByName(string name, [NotNullWhen(true)] out ItemPairing? pair)
    {
        pair = itemPairs.FirstOrDefault(x => x.Name == name);
        return pair != null;
    }
}
