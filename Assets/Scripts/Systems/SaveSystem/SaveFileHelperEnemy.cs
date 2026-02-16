using UnityEngine;

public class SaveFileHelperEnemy : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private string prefabID;

#if DEBUG
    private void OnValidate()
    {
        if (health == null) Debug.LogError("health is null", this);
        if (spawnTransform == null) Debug.LogError("spawn transform is null", this);
    }
#endif

    public DungeonSaveData.Enemy GetData()
    {
        Vector3 pos = spawnTransform.position;
        Quaternion rot = spawnTransform.rotation;

        DungeonSaveData.Enemy data = new(pos, rot, health.CurrentHealth, prefabID);

        return data;
    }
}
