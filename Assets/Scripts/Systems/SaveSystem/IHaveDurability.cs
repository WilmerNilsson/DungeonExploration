using UnityEngine;

public interface IHaveDurability
{
    public void StopSelfIntialize();

    public int Durability { get; set; }
}
