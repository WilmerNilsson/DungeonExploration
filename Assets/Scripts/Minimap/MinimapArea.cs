using System;
using System.Collections.Generic;
using UnityEngine;

public class MinimapArea : MonoBehaviour
{
    private List<MinimapPart> children = new List<MinimapPart>();
    [Tooltip("The scriptable object belonging to this level")]
    [SerializeField] private MinimapSO_test _minimapSoTest;

    private void OnValidate()
    {
        if (_minimapSoTest == null)
        {
            Debug.LogWarning("No minimap scriptable object found", this);
        }
    }


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
        ReturnDecendantOfParent(this.gameObject, children);
    }

    public void DrawArea()
    {
        foreach (MinimapPart child in children)
        {
            _minimapSoTest.AddToLists(child.prefab, child.transform.position, child.transform.localScale);
        }
    }
}
