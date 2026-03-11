using System;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TrapSoundLogic : MonoBehaviour
{
   [SerializeField] private string trapPath;
   
   public void ActivateTrap()
   {
      if (!AudioManager.IsValid) return;
      AudioManager.Instance.CreateInstance(trapPath, gameObject);
      AudioManager.Instance.StartEvent(trapPath, gameObject);
   }

   public void NextStep()
   {
      if (!AudioManager.IsValid) return;
      AudioManager.Instance.KeyOff(trapPath, gameObject);
   }
   
   public void StopAndRelease()
   {
      AudioManager.Instance.StopEvent(trapPath, STOP_MODE.ALLOWFADEOUT,gameObject);
      AudioManager.Instance.ReleaseInstance(trapPath, gameObject);
   }
}


