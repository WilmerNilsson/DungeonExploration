using System;
using UnityEngine;

public class DropItemsOnDeath : MonoBehaviour
{
    [SerializeField] private string prefabID;
    [SerializeField, Range(0f, 1f)] private float chance = 1f;
    [SerializeField] private float yOffset = 0f;
    [SerializeField] private ItemLibrarySO itemLibrary;
    [SerializeField] private Health health;

#if UNITY_EDITOR
    [SerializeField] private bool quickConnectHealth = false;

    private void OnValidate()
    {
        if(quickConnectHealth)
        {
            quickConnectHealth = false;
            if(TryGetComponent(out Health newHealth))
            {
                health = newHealth;
            }
            else
            {
                Debug.Log("found no health", this);
            }
        }

        if(itemLibrary == null)
        {
            Debug.LogWarning("item library is null", this);
        }
        else if(!itemLibrary.TryGetItemPairByName(prefabID, out _))
        {
            Debug.LogWarning($"prefab id {prefabID} not found in library", this);
        }
    }
#endif

    private void Start()
    {
        health.OnDeath.AddListener(DropItem);
    }

    private void DropItem()
    {
        if (UnityEngine.Random.value <= chance)
        {
            if (itemLibrary.TryGetItemPairByName(prefabID, out ItemPairing pair))
            {
                Vector3 instasiatePoint = transform.position;
                instasiatePoint.y += yOffset;

                Instantiate(pair.WorldPrefab, instasiatePoint, Quaternion.identity);
            }
        }
    }
}
