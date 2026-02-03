using System;
using UnityEngine;

public class HumanoidInteract : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private float interactDistance = 5f;
    private Interactable interactable;
    
    [Tooltip("The layers that will block the ray, including the interactable object")]
    [SerializeField] private LayerMask layerMask;
    private RaycastHit hit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //layerMask = LayerMask.GetMask("Interactable");
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(head.position, head.forward, out hit, interactDistance, layerMask))
        {
            if (hit.transform.gameObject.GetComponent<Interactable>())
            {
                Debug.DrawRay(head.position, head.forward * hit.distance, Color.green);
                //Debug.Log("Did Hit");
                interactable = hit.transform.gameObject.GetComponent<Interactable>();
                //UIText.SetActive(true);
            }
            else
            {
                Debug.DrawRay(head.position, head.forward * hit.distance, Color.yellow);
                //Debug.Log("Hit something else");
                interactable = null;
                //UIText.SetActive(false);
            }
        }
        else
        {
            Debug.DrawRay(head.position, head.forward * interactDistance, Color.red);
            //Debug.Log("Did not Hit");
            interactable = null;
            //UIText.SetActive(false);
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
