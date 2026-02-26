using UnityEngine;

public class SaveAndLoadButtons : MonoBehaviour
{
    public void Save()
    {
        GameManagerSO.Instance.SavefileManager.Save();
    }

    public void Load()
    {
        GameManagerSO.Instance.SavefileManager.PlaySavefile(GameManagerSO.Instance.SavefileManager.CurrentSavefileNr);
    }
}
