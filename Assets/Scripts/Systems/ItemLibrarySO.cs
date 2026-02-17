using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemMasterSO", menuName = "Scriptable Objects/ItemMasterSO")]
public class ItemLibrarySO : ScriptableObject
{
    [SerializeField] private ItemPairing[] itemPairs = { };

#if DEBUG
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
                Debug.LogWarning($"pair index {i} lacks UI prefab", this);
            }
            else if(!pair.UIPrefab.TryGetComponent<SimpleItem>(out _))
            {
                Debug.LogWarning($"pair index {i} UI prefab has no SimpleItem Script", this);
            }

            if (pair.WorldPrefab == null)
            {
                Debug.LogWarning($"pair index {i} lacks World prefab", this);
            }
            else if (!pair.WorldPrefab.TryGetComponent<ItemPickup>(out _))
            {
                Debug.LogWarning($"pair index {i} World prefab has no ItemPickup Script", this);
            }
        }
    }
#endif

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public bool TryGetItemPairByName(string name,[NotNullWhen(true)] out ItemPairing? pair)
    {
        pair = itemPairs.FirstOrDefault(x => x.Name == name);
        return pair != null;
    }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
}
