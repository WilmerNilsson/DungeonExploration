using UnityEngine;

public class SaveFileHelperPlayer : MonoBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private InventoryGrid inventoryGrid;
    [SerializeField] private Health health;

#if DEBUG
    private void OnValidate()
    {
        if (spawnTransform == null) Debug.LogWarning("spawn transform is null", this);
        if (inventoryGrid == null) Debug.LogWarning("inventory grid is null", this);
        if (health == null) Debug.LogWarning("health is null", this);
    }
#endif

    public PlayerSaveData GetData()
    {
        int currentHP = health.CurrentHealth;
        int maxHP = health.MaxHealth;
        Vector3 pos = spawnTransform.position;
        Quaternion rot = spawnTransform.rotation;
        InventorySaveData inventory = new(inventoryGrid.GetInventoryData());


        PlayerSaveData data = new(inventory, pos, rot, maxHP, currentHP, 100, 100);
        return data;
    }
}
