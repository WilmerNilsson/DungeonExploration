using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace CustomAudio
{
    [CreateAssetMenu(fileName = "AudioSystemSO", menuName = "Audio System/Audio System", order = 1)]
    public class AudioSystem : ScriptableObject
    {
        public EventList[] eventLists;
    
        private AudioSystem _instance;

        public AudioSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.LoadAll<AudioSystem>("")[0];
                }
                _instance.Initialize();
                return _instance;
            }
        }
    
        private static bool _initialized;
    
        [Serializable]
        public struct BankToLoadOnStart
        {
            public string bankName;
            public bool loadSamples;
        }
    
        public BankToLoadOnStart[] banksToLoadOnStart =
        {
            new() { bankName = "Master", loadSamples = false },
        };

        private void Initialize()
        {
            if (_initialized) return;
            foreach (var bankToLoadOnStart in banksToLoadOnStart)
            {
                BankHandler.LoadBank(bankToLoadOnStart.bankName, bankToLoadOnStart.loadSamples);
            }
            VcaHandler.Initialize();
            ParameterHandler.Initialize();
            foreach (var eventList in eventLists)
            {
                EventHandler.EventListLookup.Add(eventList.name, eventList);
                eventList.RefreshEventCache();
            }
            
            GetListener();
            CombatChecker.RefreshCombatList();
        
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        
            GameManagerSO.Instance.OnFreezeGameChangeSelfReset += OnGamePause;
        
            _initialized = true;
        }
        
#if UNITY_EDITOR
        [ContextMenu("Fill EventData")]
        public void FillAllEventData()
        {
            foreach (var list in eventLists)
            {
                list.FillEventData();
            }
        }
#endif

        private void OnGamePause(bool paused)
        {
            ParameterHandler.SetGlobalParameter("Paused", paused ? 1 : 0);
            EventHandler.PauseAllSfx(paused);
        }
    

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GetListener();
            CombatChecker.ResetCombatList();
        }
    
        private void OnSceneUnloaded(Scene scene)
        {
            _instance.CleanupInstances();
        }

        public void CleanupInstances()
        {
            foreach (var eventList in eventLists)
            {
                eventList.CleanupInstanceList(); 
            }
        }
    
        public static GameObject Listener;
        private static void GetListener()
        {
            var cameras = GameObject.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var camera in cameras)
            {
                if (!camera.TryGetComponent(typeof(StudioListener), out _)) continue;
                Listener = camera.gameObject;
                AudioDebug.Print("Successfully found listener");
                return;
            }
            AudioDebug.Print("No listener found", true);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        
        public bool debug;
        public bool showOnlyWarnings;
        
    }
    
}
