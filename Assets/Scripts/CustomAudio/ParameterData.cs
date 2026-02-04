using System;
using FMOD.Studio;

[Serializable]
public class ParameterData //Väldigt simpel, håller lite viktiga variabler för parameterCaching, används av eventData.
{
    public string paramName;
    public bool isGlobal;
    public uint data1;
    public uint data2;

    public PARAMETER_ID ID()
    {
        return new PARAMETER_ID
        {
            data1 = data1, data2 = data2
        };
    }
}
