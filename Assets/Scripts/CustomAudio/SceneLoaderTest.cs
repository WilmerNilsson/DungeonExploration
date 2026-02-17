using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderTest : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
