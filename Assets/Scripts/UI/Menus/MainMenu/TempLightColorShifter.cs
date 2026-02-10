using UnityEngine;

public class TempLightColorShifter : MonoBehaviour
{
    [SerializeField] private Light lighta;
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;
    [SerializeField] private float speed;

    // Update is called once per frame
    void Update()
    {
        float val = Mathf.Sin(Time.time * speed);
        val++;
        val /= 2f; 

        lighta.color = Color.Lerp(color1, color2, val);
    }
}
