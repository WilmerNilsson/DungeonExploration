using UnityEngine;

public class ReturnToTownButton : MonoBehaviour
{
    [SerializeField] private string townSceneName;

    public void ReturnToTown()
    {
        GameManagerSO.Instance.SavefileManager.SaveInWorld();
        GameManagerSO.Instance.MoveToScene(townSceneName);
    }
}
