using System;
using UnityEditor;
using UnityEngine;

public class AmbienceHandler : MonoBehaviour
{
   private LayerMask _layerMask;
   [SerializeField] private Transform cameraTransform;

   private void Awake()
   {
      _layerMask = LayerMask.GetMask("Walls", "Ground");
   }
   

   private RaycastHit[] _hits = new RaycastHit[8];

   private Vector3 _direction;
   
   [SerializeField] private float[] distances = new float[8]; 
   private float[] _sortedDistances = new float[8];
   
   [SerializeField] private Vector3[] directions = new Vector3[8];
   
   [SerializeField] private float medianDistance;
   
   private void FixedUpdate()
   {
      for (var i = 0; i < 8; i++)
      {
         _direction = Quaternion.AngleAxis(45 * (i + 1), cameraTransform.up) * cameraTransform.forward;
         directions[i] = _direction * 45;
         if (Physics.Raycast(cameraTransform.position, _direction, out _hits[i], Mathf.Infinity, _layerMask))
         {
            distances[i] = Vector3.Distance(cameraTransform.position, _hits[i].point);
         }
      }
      
      
   }

   private void OnDrawGizmos()
   {
      foreach (var hit in _hits)
      {
         Gizmos.DrawLine(cameraTransform.position, hit.point);
      }
   }
}
