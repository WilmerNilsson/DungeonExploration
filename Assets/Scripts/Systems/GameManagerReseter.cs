using UnityEngine;

public class GameManagerReseter : MonoBehaviour
{
    private void Awake()
    {
        GameManagerSO.Instance.ResetManagerVariables();
    }
}
