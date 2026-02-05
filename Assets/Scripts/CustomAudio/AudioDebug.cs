using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioDebug : MonoBehaviour
{
    [Serializable]
    public class InstanceList
    {
        public string EventName;
        public GameObject gameObject;
    }

    [Serializable]
    public class GlobalParamList
    {
        public string paramName;
        public float paramValue;
    }

    public List<GlobalParamList> globalParams;

    public void GetGlobalParamList()
    {
        globalParams = new List<GlobalParamList>();
        var strings = AudioManager.Instance.GetGlobalParameterList(out var values);
        foreach (var name in strings)
        {
            var tempEntry = new GlobalParamList();
            tempEntry.paramName = name;
            tempEntry.paramValue = AudioManager.Instance.GetGlobalParameterValue(name);
            globalParams.Add(tempEntry);
        }
    }
}
