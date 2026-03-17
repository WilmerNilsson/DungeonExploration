using FMOD.Studio;
using UnityEngine;

public class AmbienceHandler : MonoBehaviour
{
   [SerializeField] private LayerMask layerMask;

   private void Start()
   {
      if (!AudioManager.IsValid) return;
      AudioManager.Instance.CreateInstance(ambiencePath);
      AudioManager.Instance.StartEvent(ambiencePath);
      AudioManager.Instance.CreateInstance(slapbackPath, gameObject);
      AudioManager.Instance.StartEvent(slapbackPath, gameObject);
   }

   [SerializeField] private string ambiencePath;
   [SerializeField] private string slapbackPath;

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

   [Range(-180, 180)] 
   [SerializeField] private float minDistanceAngle;
   
   [Range(0, 1f)]
   [SerializeField] private float seekSpeed;
   private float _velocity;
   
   
   private RaycastHit[] _heightHits = new RaycastHit[2];
   
   private float _height = 0f;
   
   [SerializeField] private bool useHeightAsMultiplier;
   
   [Range(0, 3f)]
   [SerializeField] private float heightMultiplier;

   private int currentTick;
   private bool hasLoopedOnce;

   private int minIndex;
   private int maxIndex;

   private float totalDistances;
   
   private void FixedUpdate()
   {
      if (!AudioManager.Listener) return;
      
      //Öka tick
      currentTick++;
      if (currentTick >= 8)
      {
         currentTick = 0;
         hasLoopedOnce = true;
      }

      //Om vi raycastat ett varv minst en gång börja göra calculations för parametrar första ticket varje loop
      if (hasLoopedOnce && currentTick < 1)
      {
         totalDistances = 0;
         foreach (var distance in distances)
         {
            totalDistances += distance;
         }
         meanDistance = totalDistances * 0.125f;
         GetMinMax(_hits, out minIndex, out maxIndex);
         shortestDistance = _hits[minIndex].distance;
         longestDistance = _hits[maxIndex].distance;
      
         maxDistanceAngle += Mathf.DeltaAngle(maxDistanceAngle + 180, (maxIndex * 45) - 180) * seekSpeed * 0.5f;
         if (maxDistanceAngle < -180) maxDistanceAngle += 360f;
         else if (maxDistanceAngle > 180) maxDistanceAngle -= 360f;
      
         minDistanceAngle += Mathf.DeltaAngle(minDistanceAngle + 180, (minIndex * 45) - 180) * seekSpeed * 0.5f;
         if (minDistanceAngle < -180) minDistanceAngle += 360f;
         else if (minDistanceAngle > 180) minDistanceAngle -= 360f;
      
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
         
         SetParameters();
      }
      
      DoRaycast();
      if (debug) DrawRays();
   }

   private void DrawRays()
   {
      for (int i = 0; i < 8; i++)
      {
         if (i == currentTick)
         {
            Debug.DrawLine(AudioManager.Listener.transform.position, _hits[i].point, Color.blue,0);
         }
         else
         {
            Debug.DrawLine(AudioManager.Listener.transform.position, _hits[i].point, Color.white, 0);
         }
      }
   }
   
   private void DoRaycast()
   {
      //Gör raycast fram
      _flatForward = new Vector3(AudioManager.Listener.transform.forward.x, 0, AudioManager.Listener.transform.forward.z);
      _direction = Quaternion.AngleAxis(45 * currentTick, transform.up) * _flatForward;
      
      Physics.Raycast(AudioManager.Listener.transform.position, _direction, out _hits[currentTick], Mathf.Infinity, layerMask);
      
      //Lägg till nuvarande tick i distance list
      distances[currentTick] = _hits[currentTick].distance;
      directions[currentTick] = _direction * 45; //För att visualisera directions
   }

   private void SetParameters()
   {
      if (!AudioManager.IsValid) return;
      
      if (useMean)
      {
         AudioManager.Instance.SetGlobalParameter("RoomSize", meanDistance * roomSizeMultiplier * _height, false);
      }
      else
      {
         AudioManager.Instance.SetGlobalParameter("RoomSize", medianDistance * roomSizeMultiplier * _height, false);
      }
      
      AudioManager.Instance.SetGlobalParameter("ReverbPanner", maxDistanceAngle, false);
      AudioManager.Instance.SetGlobalParameter("ClosestWallDistance", _hits[minIndex].distance, false);
      AudioManager.Instance.SetGlobalParameter("DelayPanner", minDistanceAngle, false);

      currentRoomSize = meanDistance * roomSizeMultiplier * _height; //För att visualisera i inspektorn
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

   private void OnDestroy()
   {
      if (!AudioManager.IsValid) return;
      AudioManager.Instance.StopEvent(ambiencePath, STOP_MODE.ALLOWFADEOUT);
      AudioManager.Instance.ReleaseInstance(ambiencePath);
      AudioManager.Instance.StopEvent(slapbackPath, STOP_MODE.ALLOWFADEOUT, gameObject);
      AudioManager.Instance.ReleaseInstance(slapbackPath, gameObject);
   }
}
