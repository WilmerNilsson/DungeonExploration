using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] Transform bodyTransform;
    [SerializeField] Transform headTransform;
    
    private Vector3 rotationVector;

    private Quaternion targetHeadQuaternion;
    private Quaternion targetBodyQuaternion;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        headTransform.rotation = Quaternion.Lerp(headTransform.rotation, targetHeadQuaternion, 0.5f);
        bodyTransform.rotation = Quaternion.Lerp(bodyTransform.rotation, targetBodyQuaternion, 0.5f);
    }
    
    public void Rotate(Quaternion rotationQuaternion)
    {
        rotationVector = rotationQuaternion.eulerAngles;
        
        targetHeadQuaternion = rotationQuaternion;
        targetBodyQuaternion = Quaternion.AngleAxis(rotationVector.y, Vector3.up);
    }
}
