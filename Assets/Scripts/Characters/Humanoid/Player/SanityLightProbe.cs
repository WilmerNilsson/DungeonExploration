using UnityEngine;

public class SanityLightProbe : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;

    private Light[] lightsInScene;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        lightsInScene = FindObjectsByType<Light>(FindObjectsSortMode.None);
    }

    public float Sample()
    {
        float returnValue = 0f;

        foreach (Light light in lightsInScene)
        {
            if(light.isActiveAndEnabled)
            {
                float distance = Vector3.Distance(transform.position, light.transform.position);

                if (distance < light.range)
                {
                    Vector3 direction = light.transform.position - transform.position;

                    if (!Physics.Raycast(transform.position, direction, out RaycastHit hitInfo , distance, layerMask))
                    {
                        returnValue += (light.range - distance) * light.intensity; // default rendering is liniar falloff, so no reverse square
                    }
                }
            }
        }
        return returnValue;
    }
}
