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

   [Range(-180,180)]
   [SerializeField]private float maxDistanceAngle;
   
   [Range(0, 1f)]
   [SerializeField] private float seekSpeed;
   private float _velocity;
   
   
   private RaycastHit[] _heightHits = new RaycastHit[2];
   
   private float _height = 0f;
   
   [SerializeField] private bool useHeightAsMultiplier;
   
   [Range(0, 3f)]
   [SerializeField] private float heightMultiplier;
   
   private void FixedUpdate()
   {
      if (!AudioManager.Listener) return;
      for (var i = 0; i < 8; i++)
      {
         _flatForward = new Vector3(AudioManager.Listener.transform.forward.x, 0, AudioManager.Listener.transform.forward.z);
         _direction = Quaternion.AngleAxis(45 * i, transform.up) * _flatForward;
         directions[i] = _direction * 45; //För att visualisera directions
         Physics.Raycast(AudioManager.Listener.transform.position, _direction, out _hits[i], Mathf.Infinity, layerMask);
      }

      
      //OnSort(_hits);
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
      GetMinMax(_hits, out var min, out var max);
      shortestDistance = _hits[min].distance;
      longestDistance = _hits[max].distance;
      
      //Debug.DrawLine(AudioManager.Listener.transform.position, _hits[max].point, Color.red);
      
      maxDistanceAngle += Mathf.DeltaAngle(maxDistanceAngle + 180, (max * 45) - 180) * seekSpeed * 0.5f;
      if (maxDistanceAngle < -180) maxDistanceAngle += 360f;
      else if (maxDistanceAngle > 180) maxDistanceAngle -= 360f;
      
      if (useHeightAsMultiplier)
      {
         Physics.Raycast(AudioManager.Listener.transform.position, Vector3.up, out _heightHits[0], layerMask);
         Physics.Raycast(AudioManager.Listener.transform.position, Vector3.down, out _heightHits[1], layerMask);
         _height = Vector3.Distance(_heightHits[0].point, _heightHits[1].point);
         if (heightMultiplier > 0) _height *= heightMultiplier;
         else _height = 0f;
      }
      else
      {
         _height = 1;
      }
      
      if (!AudioManager.IsValid) return;
      //AudioManager.Instance.SetGlobalParameter("AmbiencePan", maxDistanceAngle, false);
      if (useMean)
      {
         AudioManager.Instance.SetGlobalParameter("RoomSize", meanDistance * roomSizeMultiplier * _height, false);
      }
      else
      {
         AudioManager.Instance.SetGlobalParameter("RoomSize", medianDistance * roomSizeMultiplier * _height, false);
      }
      
      AudioManager.Instance.SetGlobalParameter("ReverbPanner", maxDistanceAngle, false);

      currentRoomSize = meanDistance * roomSizeMultiplier * _height;
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

   private void GetMinMax(RaycastHit[] hits, out int minIndex, out int maxIndex)
   {
      var currentMin = 100f;
      var currentMinIndex = 0;
      var currentMax = 0f;
      var currentMaxIndex = 0;
      for (var i = 0; i < hits.Length; i++)
      {
         if (hits[i].distance > currentMax)
         {
            currentMax = hits[i].distance;
            currentMaxIndex = i;
         }
         else if (hits[i].distance < currentMin)
         {
            currentMin = hits[i].distance;
            currentMinIndex = i;
         }
      }
      minIndex = currentMinIndex;
      maxIndex = currentMaxIndex;
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
