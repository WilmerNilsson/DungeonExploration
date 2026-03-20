using System;
using UnityEditor.SearchService;
using UnityEngine;

[RequireComponent(typeof(IEnabledHelper))]
public class EnabledHelperCompanion : MonoBehaviour
{
    [field: SerializeField] public int UniqueID { get; private set; } = -1;

#if UNITY_EDITOR
    [SerializeField] private bool fetchIDButton;
    private void OnValidate()
    {
        if (fetchIDButton && UniqueID < 0)
        {
            EnabledHelperCompanion[] enabledHelperCompanions =
                FindObjectsByType<EnabledHelperCompanion>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            fetchIDButton = false;

            int highestNumber = 0;
            foreach(var e in enabledHelperCompanions)
            {
                if(e.UniqueID > highestNumber)
                {
                    highestNumber = e.UniqueID;
                }
            }

            UniqueID = highestNumber+1;
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
