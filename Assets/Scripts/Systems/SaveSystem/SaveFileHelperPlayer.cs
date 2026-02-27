using UnityEngine;

public class SaveFileHelperPlayer : MonoBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private HumanoidMovement movement;
    [SerializeField] private Health health;
    [SerializeField] private ItemLibrarySO itemLibrary;
    [SerializeField] private Hunger hunger;

#if DEBUG
    private void OnValidate()
    {
        if (spawnTransform == null) Debug.LogWarning("spawn transform is null", this);
        if (health == null) Debug.LogWarning("health is null", this);
        if (itemLibrary == null) Debug.LogWarning("item library is null", this);
        if (movement == null) Debug.LogWarning("movement is null", this);
        if (hunger == null) Debug.LogWarning("hunger is null", this);
    }
#endif

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
            health.SetMaxHealth(data.MaxHP);
            health.SetCurrentHealth(data.CurrentHP);

            spawnTransform.position = data.Position;
            spawnTransform.rotation = data.Rotation;
            movement.SupressMoveFrame();

            hunger.Initialize(data.Hunger);

            SaveFileHelperContainer.PopulateInventory(itemLibrary, data.Inventory, InvMasterBase.Instance.PlayerInventory);
        }

        void FromTown()
        {
            health.StopSelfInitialize();
            health.SetMaxHealth(data.MaxHP);
            health.SetCurrentHealth(data.MaxHP);

            //hunger starts at max

            SaveFileHelperContainer.PopulateInventory(itemLibrary, data.Inventory, InvMasterBase.Instance.PlayerInventory);
        }
    }

    public PlayerSaveData GetData()
    {
        int currentHP = health.CurrentHealth;
        int maxHP = health.MaxHealth;
        Vector3 pos = spawnTransform.position;
        Quaternion rot = spawnTransform.rotation;
        InventorySaveData inventory = new(InvMasterBase.Instance.PlayerInventory.GetInventoryData());
        int hungerInt = hunger.GetHungerValue();

        PlayerSaveData data = new(inventory, pos, rot, maxHP, currentHP, 100, hungerInt);
        return data;
    }
}
