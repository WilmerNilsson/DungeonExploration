using UnityEngine;
using UnityEngine.AI;

public class HallucinationSpawner : MonoBehaviour
{
    [SerializeField, Min(0f)] private float distance;
    //[SerializeField] private float spawnOffsetY;
    [SerializeField, Min(1)] private int tries;
    [SerializeField, Min(1f)] private float cycleSpeed;
    [SerializeField] private string enemySpawnID;
    [SerializeField] private EnemyLibrarySO enemyLibrary;
    [SerializeField] private int agentIndex = 0;

    private float doubleHeight = 2f;

    private void Start()
    {
        doubleHeight = NavMesh.GetSettingsByIndex(agentIndex).agentHeight * 2f;
    }

    private void OnValidate()
    {
        if(enemyLibrary == null)
        {
            Debug.LogWarning("enemy library is null", this);
        }
        else if(!enemyLibrary.TryGetPrefabByName(enemySpawnID, out _))
        {
            Debug.LogWarning("no prefab found by ID: " + enemySpawnID);
        }
    }

    private void TrySpawnOnPoint()
    {
        for(int triesLeft = tries; triesLeft > 0; triesLeft--)
        {
            float randomAngle = Random.Range(0f, 360f);
            Vector3 randomPoint = new Vector3(Mathf.Cos(randomAngle), 0f, Mathf.Sin(randomAngle)) * distance;
            randomPoint += transform.position;
            //using double the agent height was the recomended amount per documentation;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, doubleHeight, NavMesh.AllAreas))
            {
                if (enemyLibrary.TryGetPrefabByName(enemySpawnID, out GameObject prefab))
                {
                    Instantiate(prefab, hit.position, Quaternion.identity);
                }
                break;
            }
        }
    }
}
