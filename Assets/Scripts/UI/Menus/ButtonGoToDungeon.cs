using UnityEngine;

public class ButtonGoToDungeon : MonoBehaviour
{
    [SerializeField] private string SceneName;

    public void GoBackToDungeon()
    {
        GameManagerSO.Instance.SavefileManager.SaveFromTown(false, SceneName);
    }
}
