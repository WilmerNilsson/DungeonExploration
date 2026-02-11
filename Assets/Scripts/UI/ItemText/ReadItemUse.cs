using UnityEngine;

public class ReadItemUse : MonoBehaviour
{
    [SerializeField] private string text;
    [SerializeField] private bool textIsLibraryName;
    [SerializeField] private TextLibrarySO textLibrary;

#if DEBUG
    private void OnValidate()
    {
        if (text == null || text == string.Empty)
        {
            Debug.LogWarning("item text is empty", this);
        }
        else if (textIsLibraryName && textLibrary == null)
        {
            Debug.LogWarning("item description is for library, but library reference is null", this);
        }
        else if (textIsLibraryName && !textLibrary.TryGetTextByName(text, out _))
        {
            Debug.LogWarning("item description could not find text in library by name: " + text, this);
        }
    }
#endif

    public void Read()
    {
        if (textIsLibraryName)
        {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
            if (textLibrary.TryGetTextByName(text, out BookText? book))
            {
                InvMaster.Instance.OpenText(book.Text);
            }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        }
        else
        {
            InvMaster.Instance.OpenText(text);
        }
    }
}
