using UnityEngine;

public class RopeSoundLogic : MonoBehaviour
{
    [SerializeField] private string placePath;
    [SerializeField] private string climbPath;

    public void PlaceRope(bool success) //TODO ändra parameter eller nåt för place
    {
        if (!AudioManager.IsValid) return;
        if (success)
        {
            AudioManager.Instance.PlayOneShot(placePath, null, null, gameObject);
            return;
        }
        //Fail ljud finns inte än så spela inte
        //AudioManager.Instance.PlayOneShot(placePath, null, null, gameObject);
    }

    public void ClimbRope()
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.PlayOneShot(climbPath, null, null, gameObject);
    }
}
