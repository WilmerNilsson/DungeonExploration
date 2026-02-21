using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;

namespace CustomAudio
{
    [Serializable]
    public class ParameterData //Väldigt simpel, håller lite viktiga variabler för parameterCaching, används av eventData.
    {
        public string name;
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

    public static class ParameterHandler
    {
        public static Dictionary<string, ParameterData> GlobalParameters = new Dictionary<string, ParameterData>();

        public static void Initialize()
        {
            RuntimeManager.StudioSystem.getParameterDescriptionList(out var descriptionList);
            foreach (var paramDesc in descriptionList)
            {
                GlobalParameters.Add(paramDesc.name, new ParameterData
                {
                    name = paramDesc.name,
                    isGlobal = true,
                    data1 = paramDesc.id.data1,
                    data2 = paramDesc.id.data2
                });
                AudioDebug.Print("Added " + paramDesc.name + " to global parameter cache");
            }
        }
    
        public static void SetGlobalParameter(string paramName, float paramValue, bool printDebug = true)
        {
            if (GlobalParameters.TryGetValue(paramName, out var paramData))
            {
                RuntimeManager.StudioSystem.setParameterByID(paramData.ID(), paramValue);
            }
        }
    }
}