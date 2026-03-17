using UnityEngine;

[CreateAssetMenu(fileName = "UniqueIDHandlerSO", menuName = "Scriptable Objects/UniqueIDHandlerSO")]
public class UniqueIDHandlerSO : ScriptableObject
{
    [SerializeField] private int lastIDGiven;

    public int GetIDandItterate()
    {
        lastIDGiven++;
        return lastIDGiven;
    }
}
