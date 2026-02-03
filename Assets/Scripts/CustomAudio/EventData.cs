using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using GUID = FMOD.GUID;
#if UNITY_EDITOR
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

            ParameterCache = new Dictionary<string, ParameterData>(); //Fyll i parameters samt ParameterCache för att enkelt hitta en specifik ParameterData senare
            var tempParamList = new List<ParameterData>();
            foreach (var paramRef in editorEventRef.Parameters)
            {
                var tempParam = new ParameterData
                {
                    paramName = paramRef.Name,
                    isGlobal = paramRef.IsGlobal,
                    ID = paramRef.ID
                };
                tempParamList.Add(tempParam);
                ParameterCache.Add(paramRef.Name, tempParam);
            }
            parameters = tempParamList.ToArray();
            
            EditorUtils.UnloadPreviewBanks();
        }

        public void SetDebug(bool newValue) //Kallas av eventList, används för att gömma eller visa variabler i inspektorn
        {
            debug = newValue;
        }
        #endif
    }
