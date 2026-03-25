using UnityEngine;

public class WeaponSoundLogic : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    [SerializeField] private string equipPath;
    [SerializeField] private string swingPath;
    [SerializeField] private string blockPath;
    [SerializeField] private string parryPath;
    [SerializeField] private string breakPath;
    [SerializeField] private string collisionPath;
    [SerializeField] private string materialParameter;
    private string[] _materialParameters;
    private float[] _materialIndexes = new float[1];

    private void Start()
    {
        _materialParameters = new string[1] { materialParameter };
        OcclusionHandler.AddToOcclusionList(gameObject);
        if (isPlayer) PlayEquipSound();
    }

    public void PlayEquipSound()
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.PlayOneShot(equipPath, null, null, gameObject);
    }

    public void PlaySwingSound()
    {
        if(!AudioManager.IsValid) return;
        AudioManager.Instance.PlayOneShot(swingPath, null, null, gameObject);
    }

    public void PlayBlockSound()
    {
        if(!AudioManager.IsValid) return;
        AudioManager.Instance.PlayOneShot(blockPath, null, null, gameObject);
    }

    public void PlayParrySound()
    {
        if(!AudioManager.IsValid) return;
        AudioManager.Instance.PlayOneShot(parryPath, null, null, gameObject);
    }

    public void PlayBreakSound()
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.PlayOneShot(breakPath, null, null, gameObject);
    }
    
    public void OnCollision(string collisionTag, Vector3 weaponLocation, Vector3 normal)
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

    private void OnDestroy()
    {
        OcclusionHandler.RemoveFromOcclusionList(gameObject);
    }
}
