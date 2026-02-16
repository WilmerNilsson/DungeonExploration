using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    
    [SerializeField, Tooltip("Automatically detects player on Start")] Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SoundDetection()
    {
        
    }

    private void SightDetection()
    {
        
    }
}
