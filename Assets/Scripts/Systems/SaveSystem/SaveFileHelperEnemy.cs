using UnityEngine;
using UnityEngine.AI;

public class SaveFileHelperEnemy : MonoBehaviour
{
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

        if (nameEmpty) Debug.LogWarning("Helper prefab ID is empty", this);
    }
#endif

    public void Intialize(DungeonSaveData.Enemy data)
    {
        agent.Warp(data.Position);
        spawnTransform.rotation = data.Rotation;

        movement.SupressMoveFrame();

        health.StopSelfInitialize();
        health.SetCurrentHealth(data.CurrentHP);
    }

    public DungeonSaveData.Enemy? GetData()
    {
        if (health.Dead) return null;

        Vector3 pos = spawnTransform.position;
        Quaternion rot = spawnTransform.rotation;

        DungeonSaveData.Enemy data = new(pos, rot, health.CurrentHealth, prefabID);

        return data;
    }
}
