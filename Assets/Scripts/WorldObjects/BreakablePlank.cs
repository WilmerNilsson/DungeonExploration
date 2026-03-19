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
        Debug.Log("aaaaaaaaa");

        broken = true;
        gameObject.SetActive(false);
    }

    public bool IsEnabledForSave()
    {
        Debug.Log("bbbbbbbb: " + broken);

        return broken;
    }
}
