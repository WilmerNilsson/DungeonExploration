using UnityEngine;

public class screenshot : MonoBehaviour
{
    [ContextMenu("Screenshot")]
    public void ScreenShot()
    {
        ScreenCapture.CaptureScreenshot("SC.png", 4);
    }
}
