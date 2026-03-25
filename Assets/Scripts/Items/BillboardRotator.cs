using UnityEngine;

public class BillboardRotator : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.forward = transform.position - Camera.main.transform.position;
    }
}
