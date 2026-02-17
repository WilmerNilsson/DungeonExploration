using System;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    
    [SerializeField, Tooltip("Automatically detects player on Start")] Transform player;
    [SerializeField, Tooltip("Where to look/hear from")] Transform head;
    
    [Header("Vision")]
    [SerializeField] private float maxSightDistance;
    [SerializeField] private float sightAngle;
    [SerializeField] private LayerMask visionMask;

    private PlayerVisionData visionData;
    private RaycastHit[] sightHits;
    
    [Header("Sound")]
    [SerializeField] private OcclusionChecker occlusionChecker = new OcclusionChecker();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            
            player = GameObject.FindGameObjectWithTag("Player").transform;

            if (player == null)
            {
                Debug.LogWarning("Cant find Player", this);
                return;
            }

            if (player.TryGetComponent<PlayerVisionData>(out PlayerVisionData data))
            {
                visionData = data;
            }
            else
            {
                Debug.LogWarning("Cant find Vision Data on Player", this);
            }
            
        }
    }

    private void Update()
    {
        SightDetection();
    }

    private void SoundDetection()
    {
        occlusionChecker.CheckOcclusion(head.gameObject,player.gameObject,out float occlusion);
    }

    private float SightDetection()
    {
        if (Vector3.Distance(head.position, player.position) > maxSightDistance) return 0; //return if player too far away
        if (Vector3.Angle(head.forward, player.position) > sightAngle/2) return 0; // return if player outside line of sight
        
        // check how much of player is visible

        sightHits = new RaycastHit[visionData.visionSpots.Length];
        RaycastHit hit;
        float hits = 0;
        for (int i = 0; i < visionData.visionSpots.Length; i++)
        {
            if(!Physics.Raycast(head.position, visionData.visionSpots[i].position + player.position - head.position, out hit, visionMask)) continue;
            if (hit.collider.gameObject == player.gameObject)
            {
                hits++;
                sightHits[i] = hit;
            }
            
        }
        
        if (hits == 0) return 0;

        return visionData.visionSpots.Length/hits;
    }

    private void OnDrawGizmos()
    {
        if (sightHits == null) return;
        foreach (var sightHit in sightHits)
        {
            Gizmos.DrawLine(head.position, sightHit.point);
        }
    }
}
