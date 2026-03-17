using System;
using UnityEngine;

[RequireComponent(typeof(IEnabledHelper))]
public class EnabledHelperCompanion : MonoBehaviour
{
    [SerializeField] private UniqueIDHandlerSO uniqueIDHandlerSO;
    [field: SerializeField] public int UniqueID { get; private set; } = -1;

#if UNITY_EDITOR
    [SerializeField] private bool fetchIDButton;
    private void OnValidate()
    {
        if(uniqueIDHandlerSO == null)
        {
            Debug.LogWarning("unique id handler is null");
        }
        else if (fetchIDButton && UniqueID < 0)
        {
            fetchIDButton = false;

            UniqueID = uniqueIDHandlerSO.GetIDandItterate();
        }
    }
#endif

    public void EnableFromSave()
    {
        GetComponent<IEnabledHelper>().EnableFromSave();
    }

    public bool IsEnabledForSave()
    {
        return GetComponent<IEnabledHelper>().IsEnabledForSave();
    }
}
