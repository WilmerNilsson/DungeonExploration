using UnityEngine;

public class InspectItem : MonoBehaviour
{
    [SerializeField] private string text;

    public void Read()
    {
        InvMaster.Instance.OpenText(text);
    }
}
