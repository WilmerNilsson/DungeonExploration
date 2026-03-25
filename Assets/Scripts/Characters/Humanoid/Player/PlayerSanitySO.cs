using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerSanitySO", menuName = "Scriptable Objects/PlayerSanitySO")]
public class PlayerSanitySO : ScriptableObject
{
    [Tooltip("The maximum amount of hunger")]
    [SerializeField] public int MaxSanity = 100;
    public float CurrentSanity = 0;
    [Tooltip("The amount of time between hunger ticks in seconds")]
    public UnityEvent OnChangeSanity;


    public void ResetValues()
    {
        CurrentSanity = MaxSanity;
    }
    
    public void ChangeSanity(float amount)
    {
        CurrentSanity += amount;
        OnChangeSanity?.Invoke();

        if (CurrentSanity < 0)
        {
            CurrentSanity = 0;
        }
        if (CurrentSanity > MaxSanity)
        {
            CurrentSanity = MaxSanity;
        }
    }
}
