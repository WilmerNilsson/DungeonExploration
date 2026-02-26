using UnityEngine;

public class SaveAndLoadButtons : MonoBehaviour
{
    public void Save()
    {
        GameManagerSO.Instance.SavefileManager.SaveInWorld();
    }

    public void Load()
    {
        GameManagerSO.Instance.SavefileManager.PlaySavefile(GameManagerSO.Instance.SavefileManager.CurrentSavefileNr);
    }
}
