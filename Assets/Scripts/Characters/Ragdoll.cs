using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public void SetActive(bool value)
    {
        GetComponent<Animator>().enabled = !value;
        GetComponentInParent<CharacterController>().enabled = !value;
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = !value;
        }
    }
}
