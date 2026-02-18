using System;
using System.Collections.Generic;
using UnityEngine;

public class MinimapArea : MonoBehaviour
{
    private List<MinimapPart> children = new List<MinimapPart>();
    [Tooltip("The scriptable object belonging to this level")]


    public void ReturnDecendantOfParent(GameObject parent, List<MinimapPart> children)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject.TryGetComponent(out MinimapPart part))
            {
                children.Add(part);
            }
            else
            {
                ReturnDecendantOfParent(child.gameObject, children);
            }
        }
    }
    private void Awake()
    {
        //ReturnDecendantOfParent(this.gameObject, children);
    }

    public void DrawArea()
    {
        ReturnDecendantOfParent(gameObject, children);
<<<<<<< HEAD
        foreach (MinimapPart child in children)
=======
        MinimapMaster.Instance.AddToSO(children);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
>>>>>>> NewNewMain
        {
            foreach (Collider collider in GetComponents<Collider>())
            {
                collider.enabled = false;
            }
            DrawArea();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (Collider collider in GetComponents<Collider>())
            {
                collider.enabled = false;
            }
            DrawArea();
        }
    }
}
