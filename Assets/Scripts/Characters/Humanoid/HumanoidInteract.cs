using System;
using UnityEngine;
using UnityEngine.Events;

public class HumanoidInteract : MonoBehaviour
{
    [SerializeField] protected bool debug = false;
    [SerializeField] private Transform head;
    [SerializeField] private float interactDistance = 5f;
    [SerializeField] private Health health;
    private Interactable interactable;

    [Tooltip("The layers that will block the ray, including the interactable object")]
    [SerializeField] private LayerMask layerMask;
    private RaycastHit hit;

    public UnityEvent OnSee, OnUnSee;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (health == null) Debug.LogWarning("health is null", this);
        if (head == null) Debug.LogWarning("head transform is null", this);
    }
#endif


    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (!health.Dead && Physics.Raycast(head.position, head.forward, out hit, interactDistance, layerMask))
        {
            if (hit.transform.gameObject.TryGetComponent(out Interactable newInteractable))
            {
                if (debug)
                {
                    //Debug.DrawRay(head.position, head.forward * hit.distance, Color.green);
                    Debug.Log("Did Hit");
                }

                if (interactable != newInteractable)
                {
                    if (interactable)
                    {
                        interactable.OnView(false);
                    }

                    interactable = newInteractable;
                    interactable.OnView(true);
                    if (interactable.canInteract)
                    {
                        OnSee?.Invoke();
                    }
                }
                else if (!interactable.canInteract)
                {
                    OnUnSee?.Invoke();
                }
            }
            else
            {
                if (debug)
                {
                    //Debug.DrawRay(head.position, head.forward * hit.distance, Color.yellow);
                    Debug.Log("Hit something else: " + (interactable != null).ToString());
                }

                if (((object) interactable) != null)
                {
                    if (interactable) interactable.OnView(false);
                    OnUnSee?.Invoke();
                    interactable = null;
                }
            }
        }
        else
        {
            if(debug)
            {
                //Debug.DrawRay(head.position, head.forward * interactDistance, Color.red);
                Debug.Log("Did not Hit: " + (interactable != null).ToString());
            }

            if (((object) interactable) != null)
            {
                if(interactable) interactable.OnView(false);
                OnUnSee?.Invoke();
                interactable = null;
            }
        }
    }

    public void Interact()
    {
        if (interactable)
        {
            interactable.Interact();
        }
    }
}
