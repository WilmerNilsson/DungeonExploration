using UnityEngine;

public class RestingPlace : MonoBehaviour
{
    public void Rest()
    {
        MinimapMaster.Instance.SpawnMinimap();
        Hunger.instance.Eat(50);
    }
}
