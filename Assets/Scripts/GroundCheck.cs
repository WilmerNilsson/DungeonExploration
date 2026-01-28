using System;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private List<GameObject> objects;

    public bool GetIsGrounded()
    {
        return objects.Count > 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            objects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] == other.gameObject)
            {
                objects.RemoveAt(i);
                break;
            }
        }
    }
}
