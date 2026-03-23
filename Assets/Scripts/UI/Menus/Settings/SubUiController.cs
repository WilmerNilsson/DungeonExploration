using UnityEngine;

public class SubUiController : MonoBehaviour
{
#nullable enable

    [SerializeField, Tooltip("the default opened screen in inspector")] private GameObject? currentScreen;

    public void GoToScreen(GameObject newScreen)
    {
        if (currentScreen != null)
        {
            currentScreen.SetActive(false);
        }
        newScreen.SetActive(true);

        currentScreen = newScreen;
    }
}
