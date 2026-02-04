using UnityEngine;

public class BillboardRotator : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.forward = Camera.main.transform.position - transform.position;
    }
}
