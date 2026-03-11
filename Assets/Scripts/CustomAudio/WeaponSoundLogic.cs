using UnityEngine;

public class WeaponSoundLogic : MonoBehaviour
{
    [SerializeField] private string swingPath;
    [SerializeField] private string parryPath;
    [SerializeField] private string collisionPath;
    [SerializeField] private string materialParameter;
    private string[] _materialParameters;
    private float[] _materialIndexes = new float[1];

    private void Start()
    {
        _materialParameters = new string[1] { materialParameter };
    }

    public void PlaySwingSound()
    {
        if(!AudioManager.IsValid) return;
        AudioManager.Instance.PlayOneShot(swingPath, null, null, gameObject);
    }

    public void PlayParrySound()
    {
        if(!AudioManager.IsValid) return;
        AudioManager.Instance.PlayOneShot(parryPath, null, null, gameObject);
    }
    
    public void OnCollision(string collisionTag, Vector3 weaponLocation)
    {
        if(!AudioManager.IsValid) return;
        switch (collisionTag)
        {
            case "Wood":
                _materialIndexes[0] = 0;
                break;
            case "Stone":
                _materialIndexes[0] = 1;
                break;
            case "Metal":
                _materialIndexes[0] = 2;
                break;
            case "Flesh":
                _materialIndexes[0] = 3;
                break;
            default:
                return;
        }
        AudioManager.Instance.PlayOneShot(collisionPath, _materialParameters, _materialIndexes, gameObject);
    }
    
}
