using UnityEngine;

public class SaveFileHelperPlayer : MonoBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private HumanoidMovement movement;
    [SerializeField] private Health health;
    [SerializeField] private ItemLibrarySO itemLibrary;
    [SerializeField] private Hunger hunger;
    [SerializeField] private Sanity sanity;

#if DEBUG && UNITY_EDITOR
    private void OnValidate()
    {
        if (spawnTransform == null) Debug.LogWarning("spawn transform is null", this);
        if (health == null) Debug.LogWarning("health is null", this);
        if (itemLibrary == null) Debug.LogWarning("item library is null", this);
        if (movement == null) Debug.LogWarning("movement is null", this);
        if (hunger == null) Debug.LogWarning("hunger is null", this);
        if (sanity == null) Debug.Log("sanity is null", this);
    }
#endif

    public void Initialize(PlayerSaveData data)
    {
        Debug.Log("from town:" + data.FromTown);

        if(data.FromTown)
        {
            FromTown();
        }
        else
        {
            FromWorld();
        }

        void FromWorld()
        {
            health.StopSelfInitialize();
            health.SetCurrentHealth(data.CurrentHP);

            spawnTransform.position = data.Position;
            spawnTransform.rotation = data.Rotation;
            movement.SupressMoveFrame();

            hunger.Initialize(data.Hunger);
            sanity.Initialize(data.Sanity);

            SaveFileHelperContainer.PopulateInventory(itemLibrary, data.Inventory, InvMasterBase.Instance.PlayerInventory);
            SaveFileHelperContainer.PopulateInventory(itemLibrary, data.Equipment, InvMasterBase.Instance.EquipmentGrid);
        }

        void FromTown()
        {
            //health starts at max
            //hunger starts at max
            //sanity starts at max

            SaveFileHelperContainer.PopulateInventory(itemLibrary, data.Inventory, InvMasterBase.Instance.PlayerInventory);
            SaveFileHelperContainer.PopulateInventory(itemLibrary, data.Equipment, InvMasterBase.Instance.EquipmentGrid);
        }
    }

    public PlayerSaveData GetData()
    {
        int currentHP = health.CurrentHealth;
        Vector3 pos = spawnTransform.position;
        Quaternion rot = spawnTransform.rotation;
        InventorySaveData inventory = new(InvMasterBase.Instance.PlayerInventory.GetInventoryData());
        InventorySaveData equipment = new(InvMasterBase.Instance.EquipmentGrid.GetInventoryData());
        int hungerInt = hunger.GetHungerValue();
        int sanityInt = sanity.GetSanityValue();

        PlayerSaveData data = new(inventory, equipment, pos, rot, currentHP, sanityInt, hungerInt);
        return data;
    }
}
