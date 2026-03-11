using System.Collections;
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
        GameManagerSO.Instance.LockMouse(true);

        if(InvMasterBase.Instance is InvMaster master)
        {
            master.ClosePlayerInventory();
        }

        StartCoroutine(FadeToDeath());
        
        return;

        IEnumerator FadeToDeath()
        {
            yield return new WaitForSeconds(3f);
            SceneTransition.GetInstance().PlayFade(9999);
            yield return new WaitForSeconds(2f);
            toggleObject.SetActive(true);
            GameManagerSO.Instance.FreezeTime(true);
        }
    }

    public void GoToMainMenu()
    {
        GameManagerSO.Instance.MoveToScene(mainMenuSceneName);
    }
}
