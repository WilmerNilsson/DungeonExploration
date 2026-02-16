using UnityEngine;
using UnityEngine.Events;

public class RestingPlace : MonoBehaviour
{
    public UnityEvent OnRest;
    [SerializeField] private int hungerAmount;
    public void Rest()
    {
        if (!CombatChecker.IsCombat)
        {
            OnRest?.Invoke();
            MinimapMaster.Instance.SpawnMinimap();
            Hunger.instance.Eat(hungerAmount);
            gameObject.GetComponent<Interactable>().OnUnSeen.Invoke();
            Destroy(gameObject.GetComponent<Interactable>());
        }
    }
}