using UnityEngine;
using UnityEngine.Events;

public class CheckIfCombat : MonoBehaviour
{
    public UnityEvent OnCombat;
    public UnityEvent OnNoCombat;

    public void Check()
    {
        if (CombatChecker.IsCombat) OnCombat.Invoke();
        else OnNoCombat.Invoke();
    }
}
