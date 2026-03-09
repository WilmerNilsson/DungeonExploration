using UnityEngine;

public class ItemDurabilityExtraHelper : MonoBehaviour, IExtraDataHelper
{
    [SerializeField] private UIWeapon uIWeapon;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (uIWeapon == null) Debug.LogWarning("UIWeapon is null", this);
    }
#endif

    public string GetExtraData()
    {
        return uIWeapon.Durability.ToString();
    }

    public bool GiveExtraData(string json)
    {
        if (int.TryParse(json, out int result))
        {
            Debug.Log("giving extra data: " + result);

            uIWeapon.StopSelfIntialize();
            uIWeapon.Durability = result;

            return true;
        }
        else
        {
            Debug.Log("failed to parse");
            return false;
        }

    }
}
