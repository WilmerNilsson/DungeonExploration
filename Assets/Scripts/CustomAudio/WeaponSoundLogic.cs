using UnityEngine;

public class WeaponSoundLogic : MonoBehaviour
{
    [SerializeField] private string swingPath;
    [SerializeField] private string parryPath;
    [SerializeField] private string metalHitPath;
    [SerializeField] private string woodHitPath;
    [SerializeField] private string stoneHitPath;
    [SerializeField] private string fleshHitPath;

    private string _currentPath;

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
                _currentPath = woodHitPath;
                break;
            case "Stone":
                _currentPath = stoneHitPath;
                break;
            case "Metal":
                _currentPath = metalHitPath;
                break;
            case "Flesh":
                _currentPath = fleshHitPath;
                break;
        }
        AudioManager.Instance.PlayOneShot(_currentPath, null, null, gameObject);
    }
    
    
    
    //TODO: swing sounds here?
}
