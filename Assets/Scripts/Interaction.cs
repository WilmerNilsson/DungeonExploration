using System;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private float interactDistance = 5f;
    
    private LayerMask layerMask;
    private RaycastHit hit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        layerMask = LayerMask.GetMask("Interactable");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        if (Physics.Raycast(head.position, head.forward, out hit, interactDistance, layerMask))
        {
            hit.transform.gameObject.GetComponent<Interactable>().Interact();
        }
    }
}
