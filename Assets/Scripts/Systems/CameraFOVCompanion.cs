using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFOVCompanion : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Camera>().fieldOfView = GameManagerSO.Instance.SavefileManager.GlobalSettings.Fov;

        GameManagerSO.Instance.SavefileManager.GlobalSettings.OnFovChange += UpdateFov;
    }

    private void OnDisable()
    {
        GameManagerSO.Instance.SavefileManager.GlobalSettings.OnFovChange -= UpdateFov;
    }

    private void UpdateFov(int newVal)
    {
        GetComponent<Camera>().fieldOfView = newVal;
    }
}
