using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCashSO", menuName = "Scriptable Objects/PlayerCashSO")]
public class PlayerCashSO : ScriptableObject
{
    [SerializeField, Tooltip("Changes in inspector will not trigger events")] private int _currentCash;
    public int CurrentCash
    {
        get {return _currentCash; }
        private set 
        {
            _currentCash = value;
            OnCashChange?.Invoke(_currentCash);
        } 
    }

#nullable enable

    //since this is a scriptable object and not a monobehaviour, i am deliberatly not using unity events for memory leak reasons
    /// <summary>
    /// Will give the new cash amount when the changes
    /// </summary>
    public event Action<int>? OnCashChange;

    public bool CanAfford(int cost)
    {
        return CurrentCash >= cost; 
    }

    public bool TryBuy(int cost)
    {
        if(CanAfford(cost))
        {
            CurrentCash -= cost;
            return true;
        }
        return false;
    }

    public void AddCash(int cash)
    {
        CurrentCash += cash;
    }
}
