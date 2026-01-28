using UnityEngine;

public class SimpleDamage : MonoBehaviour
{
    [SerializeField, Min(1)] private int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Health health))
        {
            health.TakeDamage(damage);
        }
#if DEBUG
        else
        {
            Debug.Log("damage script triggered by collider without health", this);
            Debug.Log("collider without health: ", other);
        }
#endif
    }
}
