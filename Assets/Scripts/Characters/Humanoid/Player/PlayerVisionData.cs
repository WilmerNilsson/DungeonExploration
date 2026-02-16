using UnityEngine;

public class PlayerVisionData : MonoBehaviour
{
    [SerializeField, Tooltip("The spots on the player for enemy detection")] public Transform[] visionSpots;
}
