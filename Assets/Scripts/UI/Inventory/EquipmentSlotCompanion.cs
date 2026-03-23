using System;
using UnityEngine;

public class EquipmentSlotCompanion : MonoBehaviour
{
#nullable enable

    private HumanoidAttackAnimatorCompanion? _humanoidAAC;
    private HumanoidAttackAnimatorCompanion HumanoidAttackCompanion
    {
        get
        {
            if(_humanoidAAC == null)
            {
                _humanoidAAC = PlayerTrackerSingleton.Instance.GetComponent<HumanoidAttackAnimatorCompanion>(); ;
            }
            return _humanoidAAC;
        }
    }

    //this can be called before start
    public void OnEquip(SimpleItem item)
    {
        if(item.TryGetComponent(out UIWeapon uIWeapon) && uIWeapon.Durability > 0)
        {
            GameObject prefab = uIWeapon.GetEquipPrefab();

            if (HumanoidAttackCompanion.TryEquip(prefab, out Weapon? weaponScript))
            {
                uIWeapon.ConnectToWeapon(weaponScript);
            }
        }
        //no else since the equipment grid is supposed to also deal with normal weapons
    }

    public void OnRemove(SimpleItem item)
    {
        if (item.TryGetComponent(out UIWeapon weapon))
        {
            weapon.Unequip();
        }
        
        HumanoidAttackCompanion.Unequip();
    }
}
