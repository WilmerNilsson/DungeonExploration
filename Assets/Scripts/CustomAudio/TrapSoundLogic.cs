using FMOD.Studio;
using UnityEngine;

public class TrapSoundLogic : MonoBehaviour
{
   [SerializeField] private string trapPath;
   [SerializeField] private GameObject[] emitterObjects;
   private bool addedSelfToList;

   private void Start()
   {
      if (emitterObjects.Length == 0)
      {
         emitterObjects = new GameObject[1]{gameObject};
      }
      for (int i = 0; i < emitterObjects.Length; i++)
      {
         if (!emitterObjects[i])
         {
            if (!addedSelfToList)
                {
                    emitterObjects[i] = gameObject;
                    AudioManager.Instance.CreateInstance(trapPath, emitterObjects[i]);
                    OcclusionHandler.AddToOcclusionList(emitterObjects[i]);
                    addedSelfToList = true;
                }
           

         }
            else
            {
                AudioManager.Instance.CreateInstance(trapPath, emitterObjects[i]);
                OcclusionHandler.AddToOcclusionList(emitterObjects[i]);
            }
      }
   }
   public void ActivateTrap()
   {
      if (!AudioManager.IsValid) return;

      foreach (GameObject emitter in emitterObjects)
        {
         AudioManager.Instance.StartEvent(trapPath, emitter, true, true);
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


