using System.Collections;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField, Tooltip("The Top and Bottom Teleport points")] private Transform Top, Bottom;

    [SerializeField, Min(2), Tooltip("how long the fade lasts")] private float fadeTime = 2;
    
    private Vector3 targetPosition;
    private Vector3 PlayerPosition => PlayerTrackerSingleton.Instance.player.transform.position;
    private CharacterController PlayerController => PlayerTrackerSingleton.Instance.player.gameObject.GetComponent<CharacterController>();
    private bool foundPlayer;
    
    private float TopDistance => Vector3.Distance(PlayerPosition, Top.position);
    private float BottomDistance => Vector3.Distance(PlayerPosition, Bottom.position);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foundPlayer = PlayerTrackerSingleton.Instance != null;
        if (!foundPlayer)
        {
            Debug.Log("Rope could not find player");
        }
    }

    public void Climb()
    {
        if (foundPlayer)
        {
            // set target to the furthest TP point
            targetPosition = TopDistance < BottomDistance ? Bottom.position : Top.position;
            Debug.Log($"targetPosition: {targetPosition}, topdistance: {TopDistance}, bottomdistance: {BottomDistance}");
            // Start the fade
            SceneTransition.GetInstance().PlayFade(fadeTime);
            StartCoroutine(TpDelay());
            IEnumerator TpDelay()
            {
                yield return new WaitForSecondsRealtime(1); // 1 second as that is how long it takes for the fade to become black
                // Teleport the Player
                PlayerController.enabled = false;
                PlayerTrackerSingleton.Instance.player.transform.position = targetPosition;
                PlayerController.enabled = true;
            }
        }
        else
        {
            Debug.Log("no player");
        }
    }
}
