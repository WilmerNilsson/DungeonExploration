using UnityEngine;

public class ContainerController : MonoBehaviour
{
    //we prob want a scrriptable object with a list of all items with thier UI variants, and if so we can reference them here.
    [SerializeField] public GameObject[] spawnItems;
    [SerializeField] private InventoryGrid myGrid;
    [SerializeField] private Transform canvasTransform;

    public InventoryGrid Grid { get { return myGrid; } }

#if DEBUG
    private void OnValidate()
    {
        if (myGrid == null) Debug.LogWarning("Container lacks a inventory grid", this);
        
        foreach (GameObject item in spawnItems)
        {
            if(item == null)
            {
                Debug.LogWarning("spawn items has a null entry", this);
                continue;
            }
            if(item.GetComponent<SimpleItem>() == null)
            {
                Debug.LogWarning("spawn item lacks SimpleItemScript:" + item.name, this);
            }
        }
    }
#endif

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
            if (!couldInsert)
            {
                Debug.LogError("failed to insert spawn item: " + prefab.name, this);
            }
#else
            myGrid.TryInsertItem(item);
#endif
        }
    }

    public void Open()
    {
        if(InvMasterBase.Instance is InvMaster master)
        {
            myGrid.gameObject.SetActive(true);
            master.AddOpenWorldContainerToSystem(this);
        }
#if DEBUG
        else
        {
            Debug.LogError("can't open container cause InvMaster.Instance is not the right type", this);
        }
#endif
    }

    public void Close()
    {
        myGrid.transform.SetParent(canvasTransform, false);
        myGrid.gameObject.SetActive(false);
    }
}
