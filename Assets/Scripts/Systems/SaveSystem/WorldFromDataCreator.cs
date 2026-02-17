using UnityEngine;

#nullable enable

public class WorldFromDataCreator : MonoBehaviour
{
    private void Awake()
    {
        if(GameManagerSO.Instance.TryConsumeSavefileData(out SavefileData? data))
        {
            if(data.World != null)
            {

            }
        }
#if DEBUG

#endif

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
