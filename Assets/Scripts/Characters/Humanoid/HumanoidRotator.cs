using UnityEngine;

public class HumanoidRotator : MonoBehaviour
{
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform headTransform;
    
    [SerializeField, Range(0,1)] private float bodyRotationSpeed = .5f;
    [SerializeField, Range(0,1)] private float headRotationSpeed = .5f;
    
    private Vector3 rotationVector;

    private Quaternion targetHeadQuaternion;
    private Quaternion targetBodyQuaternion;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetHeadQuaternion = headTransform.rotation;
        targetHeadQuaternion = bodyTransform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        headTransform.rotation = Quaternion.Lerp(headTransform.rotation, targetHeadQuaternion, headRotationSpeed);
        bodyTransform.rotation = Quaternion.Lerp(bodyTransform.rotation, targetBodyQuaternion, bodyRotationSpeed);
    }
    
    public void Rotate(Quaternion rotationQuaternion)
    {
        rotationVector = rotationQuaternion.eulerAngles;
        
        targetHeadQuaternion = rotationQuaternion;
        targetBodyQuaternion = Quaternion.AngleAxis(rotationVector.y, Vector3.up);
    }
}
