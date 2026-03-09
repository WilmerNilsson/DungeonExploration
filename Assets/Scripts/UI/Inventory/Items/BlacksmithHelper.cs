using UnityEngine;

[System.Serializable]
public class BlacksmithHelper
{
    [Min(1)] public int MaxDurability;
    [Min(0)] public int CostPerDurability;
    public bool CanBeDonated;
}
