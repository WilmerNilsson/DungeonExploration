using UnityEngine;

public class DrinkItem : MonoBehaviour, IItemEffect
{
    [SerializeField] private PlayerHealthSO playerHealthSO;
    [SerializeField] private int health;

    public void Activate()
    {
        throw new System.NotImplementedException();
    }

    public string GetContextText()
    {
        return "Drink";
    }
}
