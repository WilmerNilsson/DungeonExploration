using UnityEngine;

public class HumanoidRotator : MonoBehaviour
{
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform headTransform;
    
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
        //headTransform.rotation = targetHeadQuaternion;
        bodyTransform.rotation = targetBodyQuaternion;
        headTransform.eulerAngles = new Vector3(rotationVector.x, bodyTransform.eulerAngles.y, 0);
    }
    
    public void Rotate(Quaternion rotationQuaternion)
    {
        rotationVector = rotationQuaternion.eulerAngles;

        targetHeadQuaternion = rotationQuaternion;
        targetBodyQuaternion = Quaternion.AngleAxis(rotationVector.y, Vector3.up);
    }
}
