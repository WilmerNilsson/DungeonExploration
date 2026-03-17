using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Rope : MonoBehaviour, IEnabledHelper
{
    [SerializeField, Tooltip("The Top and Bottom Teleport points")] private Transform Top, Bottom;
    [SerializeField, Tooltip("The Piton, for enable/disable")] private GameObject piton;
    [SerializeField, Tooltip("The Crack, for enable/disable")] private Collider crack;
    [SerializeField, Min(2), Tooltip("how long the fade lasts")] private float fadeTime = 2;

    public UnityEvent<bool> onTryActivateRope;
    
    private Vector3 targetPosition;
    private Vector3 PlayerPosition => PlayerTrackerSingleton.Instance.player.transform.position;
    private CharacterController PlayerController => PlayerTrackerSingleton.Instance.player.gameObject.GetComponent<CharacterController>();
    private bool foundPlayer;
    private bool isEnabled = false;

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

    public void Activate()
    {
        if (InvMasterBase.Instance.PlayerInventory.HasItemID("Rep", out SimpleItem item))
        {
            EnableRopeNoInvoke();
            InvMasterBase.Instance.DestroyItem(item);
            onTryActivateRope.Invoke(true);
        }
        else
        {
            onTryActivateRope.Invoke(false);
        }
    }

    private void EnableRopeNoInvoke()
    {
        isEnabled = true;
        Top.gameObject.SetActive(true);
        Bottom.gameObject.SetActive(true);
        piton.gameObject.SetActive(true);
        crack.enabled = false;
    }

    public void Climb()
    {
        if (foundPlayer)
        {
            // set target to the furthest TP point
            targetPosition = TopDistance < BottomDistance ? Bottom.position : Top.position;
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

    public void EnableFromSave()
    {
        EnableRopeNoInvoke();
    }

    public bool IsEnabledForSave()
    {
        return isEnabled;
    }
}
