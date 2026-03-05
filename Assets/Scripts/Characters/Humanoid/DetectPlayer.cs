using System;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    [SerializeField, Tooltip("Automatically detects player on Start")] Transform player;
    [SerializeField, Tooltip("Where to look/hear from")] Transform head;
    private HumanoidController playerController;

    [Header("Vision")] 
    [SerializeField, Tooltip("full sight cone")] private float sightAngle;
    [SerializeField] private LayerMask visionMask;

    private PlayerVisionData visionData;
    private RaycastHit[] sightHits;

    [Header("Sound")] 
    [SerializeField, Tooltip("percent modifier applied to sound when player crouches, 1 is full sound 0 is no sound"), Range(0,1)] private float crouchSoundModifier;
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
            if (player.TryGetComponent(out HumanoidController controller))
            {
                playerController = controller;
            }
            else
            {
                Debug.LogWarning("Cant find HumanoidController on Player", this);
            }

            if (player.TryGetComponent(out PlayerVisionData data))
            {
                visionData = data;
            }
            else
            {
                Debug.LogWarning("Cant find Vision Data on Player", this);
            }
        }
    }

    public bool Detect(float sightThreshold, float soundThreshold, float soundRange, float maxSightDistance)
    {
        return (SightDetection(maxSightDistance) > sightThreshold || SoundDetection(soundRange) > soundThreshold);
    }

    private float SoundDetection(float soundRange) // returns the percentage of how well the enemy can "hear" the player
    {
        if(Vector3.Distance(player.position, transform.position) > soundRange) return 0; // return if the player is too far away
        occlusionChecker.CheckOcclusion(head.gameObject,player.gameObject,out float occlusion); // run sound occlusion in reverse
        return (1-occlusion) * crouchSoundModifier;
    }

    private float SightDetection(float maxSightDistance) // returns what percentage of the player that can be seen based on the PlayerVisionData
    {
        if (Vector3.Distance(head.position, player.position) > maxSightDistance) return 0; //return if player too far away
        if (Vector3.Angle(head.forward, player.position) > sightAngle/2) return 0; // return if player outside line of sight
        
        // check how much of player is visible

        sightHits = new RaycastHit[visionData.visionSpots.Length];
        RaycastHit hit;
        float hits = 0;

        Vector3 source = RelativePosition(head.position);
        for (int i = 0; i < visionData.visionSpots.Length; i++)
        {
            Vector3 target = RelativePosition(visionData.visionSpots[i].position);
            Vector3 direction = (target-source).normalized;
            
            if(!Physics.Raycast(source, direction, out hit, visionMask)) continue;
            if (hit.collider.gameObject == player.gameObject)
            {
                hits++;
                sightHits[i] = hit;
            }
        }
        
        if (hits == 0) return 0;

        return hits/visionData.visionSpots.Length;
    }
    
    private Vector3 RelativePosition(Vector3 position)
    {
        return transform.TransformDirection(position);
    }
}
