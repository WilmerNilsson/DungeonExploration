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
      if (!AudioManager.IsValid) return;
      OcclusionHandler.AddToOcclusionList(emitterObject);
      AudioManager.Instance.CreateInstance(trapPath, emitterObject);
   }

   public void ActivateTrap()
   {
      if (!AudioManager.IsValid) return;
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
   }

   private void OnDestroy()
   {
      if (!AudioManager.IsValid) return;
      AudioManager.Instance.ReleaseInstance(trapPath, emitterObject);
      OcclusionHandler.RemoveFromOcclusionList(gameObject);
   }
}


