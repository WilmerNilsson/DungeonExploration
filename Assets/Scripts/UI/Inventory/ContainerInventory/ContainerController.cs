using UnityEngine;

public class ContainerController : MonoBehaviour
{
    //we prob want a scrriptable object with a list of all items with thier UI variants, and if so we can reference them here.
    [SerializeField] private GameObject[] spawnItems;
    [SerializeField] private InventoryGrid myGrid;

    public InventoryGrid Grid { get { return myGrid; } }

    private void Start()
    {
        foreach(GameObject prefab in spawnItems)
        {
            SimpleItem item = Instantiate(prefab).GetComponent<SimpleItem>();

#if DEBUG
            if (item == null)
            {
                Debug.LogError("chest spawn contents contain something that isn't a item", this);
                return;
            }

            bool couldInsert = myGrid.TryInsertItem(item);
            if (!couldInsert )
            {
                Debug.LogError("failed to insert spawn item", this);
            }
#else
            myGrid.TryInsertItem(item);
#endif
        }
    }

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
