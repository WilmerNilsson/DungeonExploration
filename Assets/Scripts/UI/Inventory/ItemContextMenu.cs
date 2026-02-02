using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ItemContextMenu : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField, Min(1)] private int buttonHeight;

#if DEBUG
    private void OnValidate()
    {
        if (buttonPrefab == null) Debug.LogWarning("context menu button prefab is null", this);
    }
#endif

    public void SelectItem(SimpleItem item, IItemEffect[] effects)
    {
        //can prob reuse stuff and just change text
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        transform.position = item.transform.position;

        Vector2 size = (transform as RectTransform).sizeDelta;
        size.y = buttonHeight * effects.Length;

        foreach(IItemEffect effect in effects)
        {
            GameObject newButton = Instantiate(buttonPrefab, transform);
            newButton.GetComponent<Button>().onClick.AddListener(effect.Activate);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = effect.GetContextText();
        }

    }
}
