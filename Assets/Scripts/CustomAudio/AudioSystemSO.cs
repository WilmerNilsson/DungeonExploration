using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace CustomAudio
{
    [CreateAssetMenu(fileName = "AudioSystemSO", menuName = "Audio System/Audio System", order = 1)]
    public class AudioSystem : ScriptableSingleton<AudioSystem>
    {
        public EventList[] eventLists;
    
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

        private void Awake()
        {
            Initialize();
        }

        public EventHandler EventHandler;
        public BankHandler BankHandler;
        public VcaHandler VcaHandler;
        public ParameterHandler ParameterHandler;
        
        private void Initialize()
        {
            if (_initialized) return;
            EventHandler = new EventHandler();
            BankHandler = new BankHandler();
            VcaHandler = new VcaHandler();
            ParameterHandler = new ParameterHandler();
            
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
            
            Debug.Log("AudioSystem Initialized");
        
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
            instance.CleanupInstances();
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

        public AudioSystem(VcaHandler vcaHandler, EventHandler eventHandler, BankHandler bankHandler)
        {
            this.VcaHandler = vcaHandler;
            this.EventHandler = eventHandler;
            this.BankHandler = bankHandler;
        }
    }
    
}
