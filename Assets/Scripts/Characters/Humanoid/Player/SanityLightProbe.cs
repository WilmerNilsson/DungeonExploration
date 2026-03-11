using UnityEngine;

public class SanityLightProbe : MonoBehaviour
{
    private Light[] lightsInScene;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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

                if (distance > light.range)
                {
                    //check if obstucted

                    returnValue += (light.range - distance) * light.intensity; // default rendering is liniar falloff, so no reverse square
                }
            }
        }

        return returnValue;
    }
}
