using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField, Min(1)] private int damage = 1;
    [SerializeField, Min(1)] private int durability = 1;
    [SerializeField] private bool unbreakable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Health health))
        {
            health.TakeDamage(damage);
            LoseDurability(health.DurabilityDamage);
            Debug.Log($"target health is " + health.CurrentHealth);
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
