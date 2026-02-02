using UnityEngine;

public class ThrowAwayItemEffect : MonoBehaviour, IItemEffect
{
    public void Activate()
    {
        throw new System.NotImplementedException();
    }

    public string GetContextText()
    {
        return "Drop";
    }
}
