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
        public string eventName;
        public EventReference eventReference;
        public GUID guid;
        public string[] banks;
        public bool isOneShot;
        public bool is3D;
        public bool isDoppler;
        public float minDistance;
        public float maxDistance;
        public bool debug;
        
        public EventInstance eventInstance;
        
        public ParameterData[] parameters;
        
        public Dictionary<string, ParameterData> ParameterCache;

        #if UNITY_EDITOR
        public void PopulateData()
        {
            EditorUtils.LoadPreviewBanks();
            guid = eventReference.Guid;

            if (eventName == null | eventName == "")
            {
                var split = eventReference.Path.Split('/');
                eventName = split[^1];
            }
            
            var editorEventRef = EventManager.EventFromGUID(guid);

            var tempBankList = new List<string>();
            foreach (var bank in editorEventRef.Banks)
            {
                tempBankList.Add(bank.Name);
            }
            banks = tempBankList.ToArray();
            
            EditorUtils.System.getEventByID(guid, out var eventDescription);
            
            eventDescription.isOneshot(out isOneShot);
            eventDescription.is3D(out is3D);
            eventDescription.isDopplerEnabled(out isDoppler);
            eventDescription.getMinMaxDistance(out minDistance, out maxDistance);

            ParameterCache = new Dictionary<string, ParameterData>();
            var tempParamList = new List<ParameterData>();
            foreach (var paramRef in editorEventRef.Parameters)
            {
                var tempParam = new ParameterData
                {
                    paramName = paramRef.Name,
                    isGlobal = paramRef.IsGlobal,
                    data1 = paramRef.ID.data1,
                    data2 = paramRef.ID.data2
                };
                tempParamList.Add(tempParam);
                ParameterCache.Add(paramRef.Name, tempParam);
            }
            parameters = tempParamList.ToArray();
            
            
            EditorUtils.UnloadPreviewBanks();
        }

        public void SetDebug(bool newValue)
        {
            debug = newValue;
        }
        #endif
    }
