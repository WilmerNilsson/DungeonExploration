
using TMPro;
using UnityEngine;

public class OpenBookController : MonoBehaviour
{
    private string[] pages;
    private int currentPage;
    [SerializeField] private string pageDividerIndicator = "NEXTPAGE";
    [SerializeField] private TextMeshProUGUI readingText;
    [SerializeField] private GameObject nextPageButton;
    [SerializeField] private GameObject previusPageButton;

#if DEBUG
    private void OnValidate()
    {
        if(pageDividerIndicator == null || pageDividerIndicator == string.Empty)
        {
            Debug.LogWarning("page divider string is empty", this);
        }
        if (nextPageButton == null) Debug.LogWarning("next page button is null", this);
        if (previusPageButton == null) Debug.LogWarning("previous page button is null", this);
        if (readingText == null) Debug.LogWarning("reading text field is null", this);
    }
#endif

    public void NextPage()
    {
#if DEBUG
        if(currentPage == (pages.Length-1))
        {
            Debug.LogError("can't do next page cause it is the last page already", this);
            return;
        }
#endif

        currentPage++;
        readingText.text = pages[currentPage];
        CheckButtons();
    }

    public void PreviusPage()
    {
#if DEBUG
        if (currentPage == 0)
        {
            Debug.LogError("can't do previus page cause it is the first page already", this);
            return;
        }
#endif

        currentPage--;
        readingText.text = pages[currentPage];
        CheckButtons();
    }

    private void CheckButtons()
    {
        if(currentPage == 0) previusPageButton.SetActive(false);
        else previusPageButton.SetActive(true);

        if(currentPage == (pages.Length-1)) nextPageButton.SetActive(false);
        else nextPageButton.SetActive(true);
    }

    public void OpenText(string text)
    {
        gameObject.SetActive(true);

        pages = text.Split(pageDividerIndicator);

        currentPage = 0;
        readingText.text = pages[currentPage];
        CheckButtons();
    }

    public void CloseText()
    {
        gameObject.SetActive(false);
    }
}
