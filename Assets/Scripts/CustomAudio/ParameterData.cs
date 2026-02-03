using System;
using FMOD.Studio;

[Serializable]
public class ParameterData //Väldigt simpel, håller lite viktiga variabler för parameterCaching, används av eventData.
{
    public string paramName;
    public bool isGlobal;
    public PARAMETER_ID ID;
}
