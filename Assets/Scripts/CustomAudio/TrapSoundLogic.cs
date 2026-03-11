using System;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TrapSoundLogic : MonoBehaviour
{
   [SerializeField] private string trapPath;
   private FMOD.Studio.EVENT_CALLBACK _callback;
   private EventInstance _trapSoundInstance;

   private void Start()
   {
      _callback = new EVENT_CALLBACK(ReleaseTrapInstance);
   }
   
   public void ActivateTrap()
   {
      if (!AudioManager.IsValid) return;
      AudioManager.Instance.CreateInstance(trapPath, gameObject);
      AudioManager.Instance.TryGetEventInstance(trapPath, gameObject, out _trapSoundInstance);
      _trapSoundInstance.setCallback(_callback, EVENT_CALLBACK_TYPE.STOPPED);
      AudioManager.Instance.StartEvent(trapPath, gameObject);
   }

   public void NextStep()
   {
      if (!AudioManager.IsValid) return;
      AudioManager.Instance.KeyOff(trapPath, gameObject);
   }

   [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
   private RESULT ReleaseTrapInstance(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr paramPtr)
   {
      var instance = new EventInstance(instancePtr);
      
      if (type != EVENT_CALLBACK_TYPE.STOPPED) return RESULT.OK;
      //Debug.Log("releasing");
      return instance.release();
   }
}


