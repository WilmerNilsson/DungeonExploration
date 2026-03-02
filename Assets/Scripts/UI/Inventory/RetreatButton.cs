using UnityEngine;

public class RetreatButton : MonoBehaviour
{
    [SerializeField] private string SceneName;
    public void GoBackToTown()
    {
        GameManagerSO.Instance.SavefileManager.SaveInWorld(false, SceneName);
    }
}
