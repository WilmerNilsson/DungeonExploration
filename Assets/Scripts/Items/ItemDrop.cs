using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private float cameraDropOffset;
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private SimpleItem myItem;

#if DEBUG
    private void OnValidate()
    {
        if (dropPrefab == null) Debug.LogWarning("drop prefab is null", this);
        if (myItem == null) Debug.Log("my item is null", this);
    }
#endif

    public void Drop()
    {
        Vector3 dropPos = Camera.main.transform.position;

        dropPos.y += cameraDropOffset;

        Instantiate(dropPrefab, dropPos, Quaternion.identity);

        InvMaster.Instance.DestroyItem(myItem);
    }
}
