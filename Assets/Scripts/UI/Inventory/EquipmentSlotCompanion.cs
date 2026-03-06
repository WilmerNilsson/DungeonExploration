using UnityEngine;

public class EquipmentSlotCompanion : MonoBehaviour
{
    public void OnEquip(SimpleItem item)
    {
        if(item.TryGetComponent(out UIWeapon weapon))
        {
            weapon.Equip();
        }
        //no else since the equipment grid is supposed to also deal with normal weapons
    }

    public void OnRemove(SimpleItem item)
    {
        if (item.TryGetComponent(out UIWeapon weapon))
        {
            weapon.Unequip();
        }
    }
}
