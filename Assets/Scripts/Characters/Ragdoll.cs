using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public void SetActive(bool value)
    {
        GetComponent<Animator>().enabled = !value;
        GetComponentInParent<CharacterController>().enabled = !value;
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if (rb.gameObject.layer == LayerMask.NameToLayer("Armor"))
            {
                rb.isKinematic = value;
            }
            else
            {
                rb.isKinematic = !value;
            }
        }
    }
}
