using UnityEngine;

[CreateAssetMenu(fileName = "PlayerHungerSO", menuName = "Scriptable Objects/PlayerHungerSO")]
public class PlayerHungerSO : ScriptableObject
{
    [Tooltip("The maximum amount of hunger")]
    [SerializeField] public int maxHunger = 100;
    public int currentHunger = 0;
    [Tooltip("The health component of the player")]
    public int hungerDamage = 0;
}
