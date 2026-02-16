using UnityEngine;
using UnityEngine.Events;

public class RestingPlace : MonoBehaviour
{
    public UnityEvent OnRest;
    public void Rest()
    {
        if (!CombatChecker.IsCombat)
        {
            OnRest?.Invoke();
            MinimapMaster.Instance.SpawnMinimap();
            Hunger.instance.Eat(50);
            gameObject.GetComponent<Interactable>().OnUnSeen.Invoke();
            Destroy(gameObject.GetComponent<Interactable>());
        }
    }
}