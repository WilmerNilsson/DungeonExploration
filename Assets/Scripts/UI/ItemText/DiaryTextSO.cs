using UnityEngine;

[CreateAssetMenu(fileName = "DiaryTextSO", menuName = "Scriptable Objects/DiaryTextSO")]
public class DiaryTextSO : ScriptableObject
{
    //have like a enum for page visuals.
    //perhaps have the text be a array for multiple pages you can switch between.
    [SerializeField] private string text;

    public string GetText()
    {
        return text;
    }
}
