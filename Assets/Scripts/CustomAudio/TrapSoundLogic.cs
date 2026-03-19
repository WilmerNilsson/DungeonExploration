using FMOD.Studio;
using UnityEngine;

public class TrapSoundLogic : MonoBehaviour
{
   [SerializeField] private string trapPath;
   [SerializeField] private GameObject[] emitterObjects;

   private void Start()
   {
      if (emitterObjects.Length == 0)
      {
         emitterObjects = new GameObject[1]{gameObject};
      }
      foreach (GameObject emitter in emitterObjects)
      {
         AudioManager.Instance.CreateInstance(trapPath, emitter);
      }
   }
   public void ActivateTrap()
   {
      if (!AudioManager.IsValid) return;

      foreach (GameObject emitter in emitterObjects)
        {
         OcclusionHandler.AddToOcclusionList(emitter);
         AudioManager.Instance.StartEvent(trapPath, emitter);
        }
    }

    public void NextStep()
   {
      if (!AudioManager.IsValid) return;
      foreach (var emitter in emitterObjects)
      {
         AudioManager.Instance.KeyOff(trapPath, emitter);
      }
   }
   
   public void StopAndRelease()
   {
      if (!AudioManager.IsValid) return;
      foreach (var emitter in emitterObjects)
      {
         AudioManager.Instance.StopEvent(trapPath, STOP_MODE.ALLOWFADEOUT, emitter);
      }
   }

   private void OnDestroy()
   {
      if (!AudioManager.IsValid) return;
      foreach (var emitter in emitterObjects)
      {
         AudioManager.Instance.StopEvent(trapPath, STOP_MODE.ALLOWFADEOUT, gameObject);
         AudioManager.Instance.ReleaseInstance(trapPath, emitter);
         OcclusionHandler.RemoveFromOcclusionList(emitter);
      }
   }
}


