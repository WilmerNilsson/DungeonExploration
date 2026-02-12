using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField, Min(1)] private int damage = 1;
    [SerializeField, Min(1)] private int durability = 1;
    [SerializeField] private bool unbreakable;
    [SerializeField] public bool dealDamage = true;
    private Collider body;

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
        if(dealDamage && !transform.IsChildOf(other.transform) && other.TryGetComponent(out Health health))
        {
            health.TakeDamage(damage);
            LoseDurability(health.DurabilityDamage);
            Debug.Log($"The target {other.gameObject.name} health is " + health.CurrentHealth);
        }
        if (!other.CompareTag("Player") && other.TryGetComponent(out Weapon weapon))
        {
            weapon.Interrupt();
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

    public void Interrupt()
    {
        GetComponentInParent<CrazedIK>().Interrupt();
    }
}
