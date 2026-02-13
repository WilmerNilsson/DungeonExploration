using UnityEngine;

public class RestingPlace : MonoBehaviour
{
    public void Rest()
    {
        if (!CombatChecker.IsCombat)
        {
            MinimapMaster.Instance.SpawnMinimap();
            Hunger.instance.Eat(50);
        }
    }
}