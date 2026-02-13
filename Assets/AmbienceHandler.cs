using System;
using FMOD.Studio;
using UnityEditor;
using UnityEngine;

public class AmbienceHandler : MonoBehaviour
{
   private LayerMask _layerMask;
   private Transform _cameraTransform;

   private void Awake()
   {
      _layerMask = LayerMask.GetMask("Walls", "Ground");
   }

   private void Start()
   {
      if (AudioManager.IsValid)
      {
         AudioManager.Instance.CreateInstance(AmbiencePath, gameObject);
         AudioManager.Instance.StartEvent(AmbiencePath, gameObject);
      }

      if (Camera.main != null) _cameraTransform = Camera.main.transform;
      else
      {
         Debug.LogWarning("No main camera found");
      }
   }

   [SerializeField] private string AmbiencePath;

   private RaycastHit[] _hits = new RaycastHit[8];

   private Vector3 _direction;
   
   [SerializeField] private float[] distances = new float[8]; 
   
   [SerializeField] private Vector3[] directions = new Vector3[8];

   private Vector3 _flatForward;
   
   [SerializeField] private float medianDistance;
   
   [SerializeField] private float meanDistance;
   
   [SerializeField] private float shortestDistance;
   
   [SerializeField] private float longestDistance;
   
   [Range(0.5f, 2f)][SerializeField] private float roomSizeMultiplier;
   
   private void FixedUpdate()
   {
      for (var i = 0; i < 8; i++)
      {
         _flatForward = new Vector3(_cameraTransform.forward.x, 0, _cameraTransform.forward.z);
         _direction = Quaternion.AngleAxis(45 * (i + 1), transform.up) * _flatForward;
         directions[i] = _direction * 45; //För att visualisera directions
         Physics.Raycast(_cameraTransform.position, _direction, out _hits[i], Mathf.Infinity, _layerMask);
      }
      OnSort(_hits);
      for (var i = 0; i < 8; i++)
      {
         distances[i] = _hits[i].distance;
      }
      
      medianDistance = (_hits[3].distance + _hits[4].distance) * 0.5f;
      meanDistance = (_hits[0].distance + _hits[1].distance + _hits[2].distance + _hits[3].distance + _hits[4].distance + _hits[5].distance + _hits[6].distance + _hits[7].distance) * 0.125f;
      shortestDistance = _hits[0].distance;
      longestDistance = _hits[^1].distance;

      if (AudioManager.IsValid)
      {
         AudioManager.Instance.SetGlobalParameter("RoomSize", meanDistance * roomSizeMultiplier);
      }
   }

   private void OnDrawGizmos()
   {
      foreach (var hit in _hits)
      {
         Gizmos.DrawLine(_cameraTransform.position, hit.point);
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
      AudioManager.Instance.StopEvent(AmbiencePath, STOP_MODE.ALLOWFADEOUT, gameObject);
      AudioManager.Instance.ReleaseInstance(AmbiencePath, gameObject);
   }
}
