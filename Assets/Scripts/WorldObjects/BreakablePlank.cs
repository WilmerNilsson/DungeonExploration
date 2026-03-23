using UnityEngine;

public class BreakablePlank : MonoBehaviour,  IEnabledHelper
{
    private bool broken = false;

    public void Break()
    {
        broken = true;
    }

    public void EnableFromSave()
    {
        broken = true;
        gameObject.SetActive(false);
    }

    public bool IsEnabledForSave()
    {
        return broken;
    }
}
