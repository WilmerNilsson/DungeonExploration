using UnityEngine;

public class ContainerController : MonoBehaviour
{
    //we prob want a scrriptable object with a list of all items with thier UI variants, and if so we can reference them here.
    [SerializeField] private GameObject[] spawnItems;
    [SerializeField] private InventoryGrid myGrid;

    public InventoryGrid Grid { get { return myGrid; } }

    public void Open()
    {
        myGrid.gameObject.SetActive(true);
        InvMaster.Instance.AddOpenWorldContainerToSystem(this);
    }

    public void Close()
    {
        myGrid.transform.SetParent(transform);
        myGrid.gameObject.SetActive(false);
    }
}
