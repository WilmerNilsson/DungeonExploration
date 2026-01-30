using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractTest : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float interactDistance = 5f;
    [SerializeField] private GameObject UIText;
    private Interactable interactable;

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance, layerMask))
        {
            if (hit.transform.gameObject.GetComponent<Interactable>())
            {
                Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.green);
                //Debug.Log("Did Hit");
                interactable = hit.transform.gameObject.GetComponent<Interactable>();
                UIText.SetActive(true);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.yellow);
                //Debug.Log("Hit something else");
                interactable = null;
                UIText.SetActive(false);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 1000, Color.red);
            //Debug.Log("Did not Hit");
            interactable = null;
            UIText.SetActive(false);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && interactable)
        {
            interactable.Interact();
        }
    }
}
