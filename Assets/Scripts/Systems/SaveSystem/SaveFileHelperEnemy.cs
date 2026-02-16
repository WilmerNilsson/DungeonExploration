using UnityEngine;

public class SaveFileHelperEnemy : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private string prefabID;

    public DungeonSaveData.Enemy GetData()
    {
        Vector3 pos = spawnTransform.position;
        Quaternion rot = spawnTransform.rotation;

        DungeonSaveData.Enemy data = new(pos, rot, health.CurrentHealth);

        return data;
    }
}
