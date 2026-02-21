using UnityEngine;

public class RemoveOnDeathJustForTest : MonoBehaviour
{
    public void Remove(GameObject gameObject)
    {
        CombatChecker.RemoveFromChaseList(gameObject);
    }
}
