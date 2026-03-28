using UnityEngine;

[RequireComponent(typeof(IHaveDurability))]
public class ItemDurabilityExtraHelper : MonoBehaviour, IExtraDataHelper
{
    public string GetExtraData()
    {
        return GetComponent<IHaveDurability>().Durability.ToString();
    }

    public bool GiveExtraData(string json)
    {
        if (int.TryParse(json, out int result))
        {
            IHaveDurability script = GetComponent<IHaveDurability>();

            script.StopSelfIntialize();
            script.Durability = result;

            return true;
        }
        else
        {
            Debug.Log("failed to parse durability int", this);
            return false;
        }

    }
}
