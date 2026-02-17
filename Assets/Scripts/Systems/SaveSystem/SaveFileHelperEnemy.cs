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

        bool nameEmpty = prefabID == null || prefabID == string.Empty;

        if (nameEmpty) Debug.LogWarning("Helper prefab ID is empty", this);
    }
#endif

    public void Intialize(DungeonSaveData.Enemy data)
    {
        spawnTransform.position = data.Position;
        spawnTransform.rotation = data.Rotation;

        //should prob be a set health
        health.ChangeHealth(data.CurrentHP - health.MaxHealth);
    }

    public DungeonSaveData.Enemy GetData()
    {
        Vector3 pos = spawnTransform.position;
        Quaternion rot = spawnTransform.rotation;

        DungeonSaveData.Enemy data = new(pos, rot, health.CurrentHealth, prefabID);

        return data;
    }
}
