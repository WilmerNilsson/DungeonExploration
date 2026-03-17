using System;
using UnityEngine;
using UnityEngine.Events;

public class HumanoidInteract : MonoBehaviour
{
    [SerializeField] protected bool debug = false;
    [SerializeField] private Transform head;
    [SerializeField] private float interactDistance = 5f;
    private Interactable interactable;
    
    [Tooltip("The layers that will block the ray, including the interactable object")]
    [SerializeField] private LayerMask layerMask;
    private RaycastHit hit;

    public UnityEvent OnSee, OnUnSee;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(head.position, head.forward, out hit, interactDistance, layerMask))
        {
            if (hit.transform.gameObject.TryGetComponent(out Interactable newInteractable))
            {
                if(debug)Debug.DrawRay(head.position, head.forward * hit.distance, Color.green);
                //Debug.Log("Did Hit");
                
                if (interactable != newInteractable)
                {
                    if (interactable)
                    {
                        interactable.OnView(false);
                    }
                    interactable = newInteractable;
                    interactable.OnView(true);
                    if (interactable.canInteract) OnSee?.Invoke();
                }
                else if (!interactable.canInteract)
                {
                    OnUnSee.Invoke();
                }
                //UIText.SetActive(true);
            }
            else
            {
                if(debug)Debug.DrawRay(head.position, head.forward * hit.distance, Color.yellow);
                //Debug.Log("Hit something else");
                if (interactable)
                {
                    interactable.OnView(false);
                    OnUnSee?.Invoke();
                }
                interactable = null;
                //UIText.SetActive(false);
            }
        }
        else
        {
            if(debug)Debug.DrawRay(head.position, head.forward * interactDistance, Color.red);
            //Debug.Log("Did not Hit");
            if (interactable)
            {
                interactable.OnView(false);
                OnUnSee?.Invoke();
            }
            interactable = null;
            //UIText.SetActive(false);
        }

        if (!interactable)
        {
            OnUnSee?.Invoke();
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
