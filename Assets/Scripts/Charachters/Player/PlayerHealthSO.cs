using UnityEngine;

[CreateAssetMenu(fileName = "PlayerHealthSO", menuName = "Scriptable Objects/PlayerHealthSO")]
public class PlayerHealthSO : ScriptableObject
{
    [SerializeField, Min(1)] public int MaxHealth = 5;
    [SerializeField] public int CurrentHealth = 5;
}
