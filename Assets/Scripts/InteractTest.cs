using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractTest : MonoBehaviour
{
    private LayerMask layerMask;
    [SerializeField] private float interactDistance = 5f;
    private void Awake()
    {
        layerMask = LayerMask.GetMask("Interactable");
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance, layerMask))
            {
                Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.green);
                /*Debug.Log("Did Hit");
                Debug.Log(hit.transform.gameObject.name);*/
                hit.transform.gameObject.GetComponent<Interactable>().Interact();
            }
            else
            {
                Debug.DrawRay(transform.position, transform.forward * 1000, Color.red);
                //Debug.Log("Did not Hit");
            }
        }
    }
}
