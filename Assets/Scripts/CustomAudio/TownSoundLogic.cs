using FMOD.Studio;
using UnityEngine;

public class TownSoundLogic : MonoBehaviour
{
    [Header("Town Music")]
    [SerializeField] private string townMusicPath;
    
    [Header("Town Sounds")]
    //[SerializeField] private string townAmbiencePath;
    [SerializeField] private string buyPath;
    [SerializeField] private string sellPath;
    //[SerializeField] private string bigPurchasePath;
    [SerializeField] private string doorbellPath;

    private void Start()
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.CreateInstance(townMusicPath);
        AudioManager.Instance.StartEvent(townMusicPath);
    }

    private int _location;
    
    public void ChangeLocation(string newLocation)
    {
        if (!AudioManager.IsValid) return;
        switch (newLocation)
        {
            case "Outside":
                _location = 0;
                break;
            case "Merchant":
                _location = 1;
                break;
            case "Blacksmith":
                _location = 2;
                break;
            case "Witch":
                _location = 3;
                break;
            default:
                Debug.LogWarning(newLocation + " is not a valid location");
                break;
        }
        AudioManager.Instance.SetParameter(townMusicPath, "Location", _location);
    }

    private void OnDestroy()
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.StopEvent(townMusicPath, STOP_MODE.ALLOWFADEOUT);
        AudioManager.Instance.ReleaseInstance(townMusicPath);
    }
    
    public void PlayBuySound()
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.PlayOneShot(buyPath);
    }

    public void PlaySellSound()
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.PlayOneShot(sellPath);
    }
    
    public void PlayDoorbellSound()
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.PlayOneShot(doorbellPath);
    }
}
