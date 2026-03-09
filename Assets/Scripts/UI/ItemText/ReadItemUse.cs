using UnityEngine;

public class ReadItemUse : MonoBehaviour
{
    [SerializeField] private string text;
    [SerializeField] private bool textIsLibraryName;
    [SerializeField] private TextLibrarySO textLibrary;

#nullable enable

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
        if(InvMaster.Instance is InvMaster invMaster)
        {
            if (textIsLibraryName)
            {
                if (textLibrary.TryGetTextByName(text, out BookText? book))
                {
                    invMaster.OpenText(book.Text);
                }
            }
            else
            {
                invMaster.OpenText(text);
            }
        }
    }
}
