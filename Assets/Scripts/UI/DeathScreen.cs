using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject toggleObject;
    [SerializeField] private string mainMenuSceneName;

    void Start()
    {
        GameObject.FindGameObjectWithTag("Player")?.GetComponent<Health>().OnDeath.AddListener(EnableDeathScreen);
    }

    private void EnableDeathScreen()
    {
        GameManagerSO.Instance.FreezeTime(true);
        GameManagerSO.Instance.LockMouse(true);

        if(InvMasterBase.Instance is InvMaster master)
        {
            master.ClosePlayerInventory();
        }

        toggleObject.SetActive(true);
    }

    public void GoToMainMenu()
    {
        GameManagerSO.Instance.MoveToScene(mainMenuSceneName);
    }
}
