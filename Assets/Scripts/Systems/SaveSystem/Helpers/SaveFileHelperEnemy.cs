using UnityEngine;
using UnityEngine.AI;

public class SaveFileHelperEnemy : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] float respawnChance = 0.2f;
    [field: SerializeField] public int UniqueID { get; private set; } = -1;
    [Header("Save file references")]
    [SerializeField] private Health health;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private HumanoidMovement movement;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private string prefabID;

#if DEBUG
    private void OnValidate()
    {
        if (health == null) Debug.LogError("health is null", this);
        if (spawnTransform == null) Debug.LogError("spawn transform is null", this);
        if (movement == null) Debug.LogWarning("movement is null", this);
        if (agent == null) Debug.Log("nav mesh agent is null", this);

        bool nameEmpty = prefabID == null || prefabID == string.Empty;

        if (nameEmpty && gameObject.name != "BaseAdventurer") Debug.LogWarning("Helper prefab ID is empty", this);
    }
#endif

    public bool ShouldRespawn()
    {
        return Random.value <= respawnChance;
    }

    public void SetID(int newID)
    {
        UniqueID = newID;
    }

    public void Intialize(DungeonSaveData.Enemy data)
    {
        agent.Warp(data.Position);
        spawnTransform.rotation = data.Rotation;
        UniqueID = data.UniqueID;

        movement.SupressMoveFrame();

        health.StopSelfInitialize();
        health.SetCurrentHealth(data.CurrentHP);
    }

    public DungeonSaveData.Enemy? GetData()
    {
        if (health.Dead) return null;

        Vector3 pos = spawnTransform.position;
        Quaternion rot = spawnTransform.rotation;

        DungeonSaveData.Enemy data = new(UniqueID, pos, rot, health.CurrentHealth, prefabID);

        return data;
    }
}
