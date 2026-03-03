using UnityEngine;
using UnityEngine.AI;

public class HallucinationSpawner : MonoBehaviour
{
    [SerializeField, Min(0f)] private float distance;
    //[SerializeField] private float spawnOffsetY;
    [SerializeField, Min(1)] private int tries = 5;
    [SerializeField, Min(1f)] private float minSpawnTime;
    [SerializeField, Min(1f)] private float maxSpawnTime;
    [SerializeField, Min(0), Tooltip("the maximum sanity value, where spawns starts happening")]
    private int sanityMax = 40;
    [SerializeField, Min(0), Tooltip("the minimum sanity value, where spawns are on max")]
    private int sanityMin = 0;
    [Header("nightmares")]
    [SerializeField] private string nightmareSpawnID;
    [SerializeField, Range(0f,1f)] private float minNightmareSpawnPercent;
    [SerializeField, Range(0f, 1f)] private float maxNightmareSpawnPercent = 1f;
    [SerializeField, Min(0), Tooltip("the maximum sanity value, where nightmare spawns starts happening")]
    private int sanityMaxNightmare = 40;
    [SerializeField, Min(0), Tooltip("the minimum sanity value, where nightmare spawns are on max")]
    private int sanityMinNightmare = 0;
    [Header("needed fields")]
    [SerializeField] private string hallucinationSpawnID;
    [SerializeField] private EnemyLibrarySO enemyLibrary;
    [SerializeField] private int agentIndex = 0;
    [SerializeField] private Sanity mySanity;

    private float doubleHeight = 2f;

    private float timeLeftToSpawnPercent = 1f;

#if DEBUG
    private void OnValidate()
    {
        if (enemyLibrary == null)
        {
            Debug.LogWarning("enemy library is null", this);
        }
        else
        {
            if (hallucinationSpawnID != null && !enemyLibrary.TryGetPrefabByName(hallucinationSpawnID, out _))
            {
                Debug.LogWarning("no prefab found by ID: " + hallucinationSpawnID, this);
            }
            else if (hallucinationSpawnID == null || hallucinationSpawnID == string.Empty)
            {
                Debug.LogWarning("hallucination spawn id is null or empty", this);
            }

            if (nightmareSpawnID != null && !enemyLibrary.TryGetPrefabByName(nightmareSpawnID, out _))
            {
                Debug.LogWarning("no prefab found by ID: " + nightmareSpawnID, this);
            }
            else if (nightmareSpawnID == null || nightmareSpawnID == string.Empty)
            {
                Debug.LogWarning("nightmare spawn id is null or empty", this);
            }
        }

        if (mySanity == null) Debug.LogWarning("sanity is null", this);

        if(sanityMin > sanityMax)
        {
            Debug.LogWarning("sanity max spawn rate is before they start spawning, correcting", this);
            sanityMin = sanityMax;
        }
    }
#endif

    //to get a smoother gradient i calculate it every frame
    private void Update()
    {
        if (sanityMax < mySanity.CurrentSanity)
        {
            timeLeftToSpawnPercent = 1f;
            return;
        }
        //starts at 1 and goes down, higher sanity means less spawns
        float reverseStrength = (float)(mySanity.CurrentSanity - sanityMin) / (float)(sanityMax -  sanityMin); 
        float currentSpawnTime = Mathf.Lerp(minSpawnTime, minSpawnTime, reverseStrength);

        float fraction = 1f / currentSpawnTime;
        fraction *= Time.deltaTime;

        timeLeftToSpawnPercent -= fraction;
        if(timeLeftToSpawnPercent <= 0)
        {
            timeLeftToSpawnPercent = 1f; //if player gets a lagspike it won't make two spawn closer to eachother
            TrySpawn();
        }
    }

    private string ChoosePrefabID()
    {
        float reverseStrength = (float)(mySanity.CurrentSanity - sanityMinNightmare) / (float)(sanityMaxNightmare - sanityMinNightmare);
        Debug.Log("reverse Strength: " + reverseStrength);

        float nightmareSpawnPercent = Mathf.Lerp(maxNightmareSpawnPercent, minNightmareSpawnPercent, reverseStrength);

        Debug.Log("choosing: " + nightmareSpawnPercent);

        if(Random.value <= nightmareSpawnPercent)
        {
            return nightmareSpawnID;
        }
        else
        {
            return hallucinationSpawnID;
        }
    }

    private void Start()
    {
        doubleHeight = NavMesh.GetSettingsByIndex(agentIndex).agentHeight * 2f;
    }

    private void TrySpawn()
    {
        //we could prob just offsett the rotation each try to get a guaranteed wide coverage
        for(int triesLeft = tries; triesLeft > 0; triesLeft--)
        {
            float randomAngle = Random.Range(0f, 360f);
            Vector3 randomPoint = new Vector3(Mathf.Cos(randomAngle), 0f, Mathf.Sin(randomAngle)) * distance;
            randomPoint += transform.position;
            //using double the agent height was the recomended amount per documentation;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, doubleHeight, NavMesh.AllAreas))
            {
                if (enemyLibrary.TryGetPrefabByName(ChoosePrefabID(), out GameObject prefab))
                {
                    Instantiate(prefab, hit.position, Quaternion.identity);
                }
                break;
            }
#if DEBUG
            if (triesLeft == 0) Debug.Log("failed to spawn hallucination", this);
#endif
        }
    }
}
