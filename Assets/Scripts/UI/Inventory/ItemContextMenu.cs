using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ItemContextMenu : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField, Min(1)] private int buttonHeight;

    private SimpleItem selectedItem;

#if DEBUG
    private void OnValidate()
    {
        if (buttonPrefab == null) Debug.LogWarning("context menu button prefab is null", this);
    }
#endif

    public bool TryDeselectItem(SimpleItem item)
    {
        if(selectedItem = item)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            return true;
        }
        return false;
    }

    public void SelectItem(SimpleItem item, ItemUse[] effects)
    {
        selectedItem = item;

        //can prob reuse stuff and just change text
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        transform.position = item.transform.position;

        Vector2 size = (transform as RectTransform).sizeDelta;
        size.y = buttonHeight * effects.Length;
        (transform as RectTransform).sizeDelta = size;

        foreach (ItemUse effect in effects)
        {
            GameObject newButton = Instantiate(buttonPrefab, transform);
            newButton.GetComponent<Button>().onClick.AddListener(effect.Activate);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = effect.GetText();
        }

    }
}
