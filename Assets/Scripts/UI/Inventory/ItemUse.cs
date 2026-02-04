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

    public void Activate()
    {
        OnUse.Invoke(); 
    }
}
