using UnityEngine;

public class EquipmentSlotCompanion : MonoBehaviour
{
    public void OnEquip(SimpleItem item)
    {
        if(item.TryGetComponent(out UIWeapon weapon))
        {

        }
        //no else since the equipment grid is supposed to also deal with normal weapons
    }
}
