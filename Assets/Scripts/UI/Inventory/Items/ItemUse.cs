using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ItemUse
{
    [SerializeField] private string text;
    [SerializeField] public UnityEvent OnUse;

    public string GetText()
    {  
        return text;
    }

#if UNITY_EDITOR
    public void SetText(string text)
    {
        this.text = text;
    }
#endif

    public void Activate()
    {
        OnUse.Invoke(); 
    }
}
