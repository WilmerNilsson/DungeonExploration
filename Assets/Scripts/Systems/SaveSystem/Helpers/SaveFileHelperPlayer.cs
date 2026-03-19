using UnityEngine;

public class SaveFileHelperPlayer : MonoBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private HumanoidMovement movement;
    [SerializeField] private Health health;
    [SerializeField] private ItemLibrarySO itemLibrary;
    [SerializeField] private Sanity sanity;
    [SerializeField] private int runCount;
    [SerializeField] private string startingWeaponID;

#if DEBUG && UNITY_EDITOR
    private void OnValidate()
    {
        if (spawnTransform == null) Debug.LogWarning("spawn transform is null", this);
        if (health == null) Debug.LogWarning("health is null", this);
        if (itemLibrary == null) Debug.LogWarning("item library is null", this);
        if (movement == null) Debug.LogWarning("movement is null", this);
        if (sanity == null) Debug.Log("sanity is null", this);

        if(startingWeaponID == null && startingWeaponID == string.Empty)
        {
            Debug.LogWarning("starting weapon ID is null", this);
        }
        else if (itemLibrary != null)
        {
            if(!itemLibrary.TryGetItemPairByName(startingWeaponID, out _))
            {
                Debug.LogWarning($"no item by name {startingWeaponID} found", this);
            }
        }
    }
#endif

    public void InitializeNew()
    {
        if (itemLibrary.TryGetItemPairByName(startingWeaponID, out ItemPairing pair))
        {
            InvMasterBase.Instance.EquipmentGrid.TryInsertItem(pair.UIPrefab.GetComponent<SimpleItem>(), true);
        }
        else
        {
            Debug.LogWarning("could not find starting weapon id", this);
        }
    }

    public void Initialize(PlayerSaveData data)
    {
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

            sanity.Initialize(data.Sanity);
            runCount = data.RunCount;

            SaveFileHelperContainer.PopulateInventory(itemLibrary, data.Inventory, InvMasterBase.Instance.PlayerInventory);
            SaveFileHelperContainer.PopulateInventory(itemLibrary, data.Equipment, InvMasterBase.Instance.EquipmentGrid);
        }

        void FromTown()
        {
            //health starts at max
            //hunger starts at max
            //sanity starts at max
            runCount = data.RunCount;

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
        int sanityInt = sanity.GetSanityValue();

        PlayerSaveData data = new(inventory, equipment, pos, rot, currentHP, sanityInt, runCount);
        return data;
    }
    
    public void DropItems()
    {
        Vector3 spawnPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<HumanoidMovement>().lastGroundedPosition;
        if (InvMasterBase.Instance.PlayerInventory.GetInventoryData().Count <= 0 && InvMasterBase.Instance.EquipmentGrid.GetInventoryData().Count <= 0)
        {
            return;
        }
        InventorySaveData inventory = new(InvMasterBase.Instance.PlayerInventory.GetInventoryData());
        for (int i = 0; i < inventory.Items.Count; i++)
        {
            itemLibrary.TryGetItemPairByName(inventory.Items[i].PrefabID, out ItemPairing pair);
            Instantiate(pair.WorldPrefab, spawnPosition, Quaternion.identity);
        }
        
        InventorySaveData equipment = new(InvMasterBase.Instance.EquipmentGrid.GetInventoryData());
        for (int i = 0; i < equipment.Items.Count; i++)
        {
            itemLibrary.TryGetItemPairByName(equipment.Items[i].PrefabID, out ItemPairing pair);
            Instantiate(pair.WorldPrefab, spawnPosition, Quaternion.identity);
        }

        InvMasterBase.Instance.PlayerInventory.EmptyInventory();
        InvMasterBase.Instance.EquipmentGrid.EmptyInventory();
    }
}
