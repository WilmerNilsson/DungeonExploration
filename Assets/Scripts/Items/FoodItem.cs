using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [SerializeField, Min(0)] private int amount = 3;
    [SerializeField] private SimpleItem myItem;

#if DEBUG
    private void OnValidate()
    {
        if (myItem == null) Debug.LogWarning("my item is null", this);
    }
#endif
    public void Eat()
    {
        Hunger.instance.Eat(amount);
        InvMaster.Instance.DestroyItem(myItem);
    }
}
