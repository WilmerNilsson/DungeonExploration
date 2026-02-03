using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DiaryTextSetter : MonoBehaviour
{
    public void SetText(DiaryTextSO diaryText)
    {
        GetComponent<TextMeshProUGUI>().text = diaryText.GetText();
    }
}
