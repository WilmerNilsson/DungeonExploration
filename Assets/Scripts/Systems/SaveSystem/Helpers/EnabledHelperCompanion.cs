using UnityEngine;

[RequireComponent(typeof(IEnabledHelper))]
public class EnabledHelperCompanion : MonoBehaviour
{
    public void EnableFromSave()
    {
        GetComponent<IEnabledHelper>().EnableFromSave();
    }

    public bool IsEnabledForSave()
    {
        return GetComponent<IEnabledHelper>().IsEnabledForSave();
    }
}
