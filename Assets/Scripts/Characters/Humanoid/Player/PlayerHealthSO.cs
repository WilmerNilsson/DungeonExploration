using UnityEngine;

[CreateAssetMenu(fileName = "PlayerHealthSO", menuName = "Scriptable Objects/PlayerHealthSO")]
public class PlayerHealthSO : ScriptableObject
{
    [SerializeField, Min(1)] public int MaxHealth = 5;
    [SerializeField] public int CurrentHealth = 5;
}

public struct HealthData
{
    public int CurrentHealth;
    public int MaxHealth;

    public HealthData(int currentHealth, int maxHealth)
    {
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
    }
}