using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioDebug : MonoBehaviour
{
    public enum Procedures
    {
        GetGlobalParameterList,
        GetInstanceList,
        GetLocalParameterList,
        GetAllInstances,
        GetAllVcas,
        GetLoadedBanks,
    }

    public string path;
    public Procedures procedure;
    public string text;
}
