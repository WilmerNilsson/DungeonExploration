using UnityEngine;

public class SimpleDamage : MonoBehaviour
{
    [SerializeField, Min(1)] private int damage = 1;

#nullable enable

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Health health))
        {
            health.TakeDamage(damage);
            if (transform.parent.TryGetComponent(out AttackPlayer? attackPlayer))
            {
                attackPlayer!.LoseDurability(health.DurabilityDamage);
            }
        }
        else
        {
            Debug.Log("damage script triggered by collider without health", this);
            Debug.Log("collider without health: ", other);
        }
    }
}
