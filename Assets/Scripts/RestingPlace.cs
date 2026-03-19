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
            gameObject.GetComponent<Interactable>().OnUnSeen.Invoke();
            Destroy(gameObject.GetComponent<Interactable>());
        }
    }
}