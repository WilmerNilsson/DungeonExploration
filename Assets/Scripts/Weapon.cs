using System;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [SerializeField, Min(1)] private int damage = 1;
    [SerializeField, Min(1)] private int durability = 1;
    [SerializeField] private bool unbreakable;
    [SerializeField] public bool dealDamage = true;
    [SerializeField] private bool unblockable = false;
    private Collider body;
    
    public UnityEvent onDamage;
    public UnityEvent onBlocked;

    private void OnEnable()
    {
        body = GetComponent<Collider>();
    }

    public void SetActive(bool value)
    {
        body.enabled = value;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter {other.gameObject.name}");
        if(dealDamage && !transform.IsChildOf(other.transform) && other.TryGetComponent(out Health health))
        {
            onDamage?.Invoke();
            health.TakeDamage(damage);
            LoseDurability(health.DurabilityDamage);
            SetActive(false);
            Debug.Log($"The target {other.gameObject.name} health is " + health.CurrentHealth);
        }
        if (!unblockable && other.TryGetComponent(out Weapon weapon))
        {
            onBlocked?.Invoke();
        }
    }
    
    public void LoseDurability(int amount)
    {
        if (unbreakable)
        {
            return;
        }
        durability -= amount;
        if (durability <= 0)
        {
            Destroy(gameObject);
        }
    }
}
