using FMOD.Studio;
using UnityEngine;

public class AmbienceHandler : MonoBehaviour
{
   [SerializeField] private LayerMask layerMask;

   private void Start()
   {
      if (!AudioManager.IsValid) return;
      AudioManager.Instance.CreateInstance(ambiencePath, gameObject);
      AudioManager.Instance.StartEvent(ambiencePath, gameObject);
   }

   [SerializeField] private string ambiencePath;

   private RaycastHit[] _hits = new RaycastHit[8];

   private Vector3 _direction;
   
   [SerializeField] private float[] distances = new float[8]; 
   
   [SerializeField] private Vector3[] directions = new Vector3[8];

   private Vector3 _flatForward;
   
   [SerializeField] private float medianDistance;
   
   [SerializeField] private float meanDistance;
   
   [SerializeField] private float shortestDistance;
   
   [SerializeField] private float longestDistance;

   [SerializeField] private bool useMean;

   [SerializeField] private bool debug;
   
   [Range(0.5f, 2f)][SerializeField] private float roomSizeMultiplier;
   
   [SerializeField] private float currentRoomSize;
   
   private void FixedUpdate()
   {
      if (!AudioManager.Listener) return;
      for (var i = 0; i < 8; i++)
      {
         _flatForward = new Vector3(AudioManager.Listener.transform.forward.x, 0, AudioManager.Listener.transform.forward.z);
         _direction = Quaternion.AngleAxis(45 * (i + 1), transform.up) * _flatForward;
         directions[i] = _direction * 45; //För att visualisera directions
         Physics.Raycast(AudioManager.Listener.transform.position, _direction, out _hits[i], Mathf.Infinity, layerMask);
      }
      OnSort(_hits);
      for (var i = 0; i < 8; i++)
      {
         distances[i] = _hits[i].distance;
      }
      
      medianDistance = (_hits[3].distance + _hits[4].distance) * 0.5f;
      float totalDistances = 0;
      for (var i = 0; i < 8; i++)
      {
         totalDistances += distances[i];
      }
      meanDistance = totalDistances * 0.125f;
      shortestDistance = _hits[0].distance;
      longestDistance = _hits[^1].distance;

      if (AudioManager.IsValid)
      {
         if (useMean)
         {
            AudioManager.Instance.SetGlobalParameter("RoomSize", meanDistance * roomSizeMultiplier);
            currentRoomSize = meanDistance * roomSizeMultiplier;
         }
         else
         {
            AudioManager.Instance.SetGlobalParameter("RoomSize", medianDistance * roomSizeMultiplier);
            currentRoomSize = medianDistance * roomSizeMultiplier;
         }
      }
   }

   private void OnDrawGizmos()
   {
      if (!Application.isPlaying || !debug) return;
      if (!AudioManager.Listener) return;
      foreach (var hit in _hits)
      {
         Gizmos.DrawLine(AudioManager.Listener.transform.position, hit.point);
      }
   }

   private void OnSort(RaycastHit[] hits)
   {
      for (int i = 0; i < hits.Length - 1; i++)
      {
         RaycastHit temp = _hits[i];
         int min = i;
         for (int j = i + 1; j < hits.Length; j++)
         {
            if (hits[j].distance < temp.distance)
            {
               temp = hits[j];
               min = j;
            }
         }
         hits[min] = hits[i];
         hits[i] = temp;
      }
   }

   private void OnDestroy()
   {
      if (!AudioManager.IsValid) return;
      AudioManager.Instance.StopEvent(ambiencePath, STOP_MODE.ALLOWFADEOUT, gameObject);
      AudioManager.Instance.ReleaseInstance(ambiencePath, gameObject);
   }
}
