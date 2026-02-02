using System;
using FMOD.Studio;

[Serializable]
public class ParameterData
{
    public string paramName;
    public bool isGlobal;
    public uint data1;
    public uint data2;

    public PARAMETER_ID ID()
    {
        return new PARAMETER_ID() { data1 = data1, data2 = data2 };
    }
}
