using UnityEngine;

[System.Serializable]
public class MadMelee : MadState
{
    [SerializeField, Tooltip("max distance to target before Chasing")] private float maxMeleeRange;
    [SerializeField, Tooltip("minimum distance to target")] private float minMeleeRange;
}
