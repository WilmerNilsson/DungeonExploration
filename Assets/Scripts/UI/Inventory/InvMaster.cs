using UnityEngine;


public class InvMaster : MonoBehaviour
{
    public static InvMaster GetInvMasterInstance;
    [SerializeField, Min(1)] private int collumns = 1;
    [SerializeField, Min(1)] private int rows = 1;
    [SerializeField] private RectTransform inventoryGrid;
}
