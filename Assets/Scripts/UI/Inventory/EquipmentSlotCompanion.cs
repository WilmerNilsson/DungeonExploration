using UnityEngine;

public class EquipmentSlotCompanion : MonoBehaviour
{
    [SerializeField] private HumanoidAttackAnimatorCompanion humanoidAttackCompanion;

#nullable enable

#if UNITY_EDITOR
    [SerializeField] private bool quickConnectCompanion = false;
    private void OnValidate()
    {
        if(quickConnectCompanion)
        {
            quickConnectCompanion = false;

            HumanoidAttackAnimatorCompanion? a = GameObject.FindAnyObjectByType<HumanoidAttackAnimatorCompanion>();

            if(a != null)
            {
                humanoidAttackCompanion = a;
            }
            else
            {
                Debug.Log("could not find HumanoidAttackAnimatorCompanion in scene", this);
            }
        }

        if (!UnityEditor.EditorUtility.IsPersistent(gameObject) && humanoidAttackCompanion == null)
            Debug.LogWarning("attack companion is null", this);
    }
#endif

    public void OnEquip(SimpleItem item)
    {
        if(item.TryGetComponent(out UIWeapon uIWeapon))
        {
            GameObject prefab = uIWeapon.GetEquipPrefab();

            if (humanoidAttackCompanion.TryEquip(prefab, out Weapon? weaponScript))
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
    }
}
