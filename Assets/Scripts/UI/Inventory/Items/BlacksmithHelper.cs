using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class BlacksmithHelper
{
    [SerializeField, FormerlySerializedAs("CostPerDurability"), Min(0)] public int Cost;
    [SerializeField, Tooltip("Wether or not it is per durability or a straight cost")] public bool CostIsPerDurability;
    public bool CanBeDonated;
}
