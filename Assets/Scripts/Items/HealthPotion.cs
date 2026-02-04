using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [SerializeField, Min(0)] private int amount = 3;
    public void Drink()
    {
        Health.PlayerHealthInstance.ChangeHealth(amount);
    }
}
