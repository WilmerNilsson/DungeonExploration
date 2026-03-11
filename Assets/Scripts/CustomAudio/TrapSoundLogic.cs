using System;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TrapSoundLogic : MonoBehaviour
{
   [SerializeField] private string trapPath;
   [SerializeField] private GameObject emitterObject;

   private void Start()
   {
      if (!emitterObject)
      {
         emitterObject = gameObject;
      }
   }

   public void ActivateTrap()
   {
      if (!AudioManager.IsValid) return;
      AudioManager.Instance.CreateInstance(trapPath, emitterObject);
      AudioManager.Instance.StartEvent(trapPath, emitterObject);
   }

   public void NextStep()
   {
      if (!AudioManager.IsValid) return;
      AudioManager.Instance.KeyOff(trapPath, emitterObject);
   }
   
   public void StopAndRelease()
   {
      AudioManager.Instance.StopEvent(trapPath, STOP_MODE.ALLOWFADEOUT,emitterObject);
      AudioManager.Instance.ReleaseInstance(trapPath, emitterObject);
   }
}


