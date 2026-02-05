using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using GUID = FMOD.GUID;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
    public class EventData
    {
        //Viktiga variabler (metadata) för event. eventName och eventReference bestäms i inspektorn. Resten av PopulateData().
        public string eventName;
        public EventReference eventReference;
        public string[] banks;
        public bool isOneShot;
        public bool is3D;
        public float minDistance;
        public float maxDistance;
        public bool debug;
        
        public EventInstance eventInstance;
        
        public ParameterData[] parameters;
        
        public Dictionary<string, ParameterData> ParameterCache; //För snabbare lookup i ParameterData istället för foreach loop

        public bool TryGetParamData(string paramName, out ParameterData paramData)
        {
            if (ParameterCache.TryGetValue(paramName, out paramData))
            {
                return true;
            }
            paramData = null;
            return false;
        }

        public void RefreshParameterCache()
        {
            ParameterCache = new Dictionary<string, ParameterData>();
            foreach (var parameterData in parameters)
            {
                ParameterCache.Add(parameterData.paramName, parameterData);
            }
        }

        #if UNITY_EDITOR
        public void PopulateData()
        {
            EditorUtils.LoadPreviewBanks(); //Behövs för att EventManager och EditorUtils ska fungera

            if (eventName == null | eventName == "") //Fyll eventName om den inte redan har ett namn
            {
                var split = eventReference.Path.Split('/');
                eventName = split[^1];
            }
            
            //EditorEventRefs är väldigt användbara eftersom de håller väldigt mycket metadata om ett event.
            //Eftersom de bara kan nås i Editorn behöver vi själva kopiera vissa variabler,
            //som vilka banker ett event tillhör samt vilka parametrar som används av eventet.
            
            var editorEventRef = EventManager.EventFromGUID(eventReference.Guid); 

            var tempBankList = new List<string>(); 
            foreach (var bank in editorEventRef.Banks)
            {
                tempBankList.Add(bank.Name);
            }
            banks = tempBankList.ToArray();
            
            isOneShot = editorEventRef.IsOneShot;
            is3D = editorEventRef.Is3D;
            minDistance = editorEventRef.MinDistance;
            maxDistance = editorEventRef.MaxDistance;

             //Fyll i parameters
            var tempParamList = new List<ParameterData>();
            foreach (var paramRef in editorEventRef.Parameters)
            {
                var tempParam = new ParameterData
                {
                    paramName = paramRef.Name,
                    isGlobal = paramRef.IsGlobal,
                    data1 = paramRef.ID.data1,
                    data2 = paramRef.ID.data2,
                };
                tempParamList.Add(tempParam);
            }
            parameters = tempParamList.ToArray();
            
            EditorUtils.UnloadPreviewBanks();
        }
        #endif
        public void SetDebug(bool newValue) //Kallas av eventList, används för att gömma eller visa variabler i inspektorn
        {
            debug = newValue;
        }
       
    }
