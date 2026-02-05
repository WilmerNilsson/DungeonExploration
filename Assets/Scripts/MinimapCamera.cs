using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float height = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, target.transform.position.y + height, transform.position.z);
    }
}
