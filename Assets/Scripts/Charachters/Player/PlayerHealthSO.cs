using UnityEngine;

[CreateAssetMenu(fileName = "PlayerHealthSO", menuName = "Scriptable Objects/PlayerHealthSO")]
public class PlayerHealthSO : ScriptableObject
{
    [SerializeField] public int MaxHealth;
    [SerializeField] public int CurrentHealth;
}
