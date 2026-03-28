using System;
using TMPro;
using UnityEngine;

public class UIWeapon : MonoBehaviour, IHaveDurability
{
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private TextMeshProUGUI durabilityText;
    [SerializeField] public BlacksmithHelper BlacksmithHelper;


#nullable enable
    public int Durability
    {
        get
        {
            if(worldWeapon == null)
            {
                return _durability;
            }
            else
            {
                return worldWeapon.Durability;
            }
        }
        set
        {
            if (worldWeapon == null)
            {
                _durability = value;
            }
            else
            {
                worldWeapon.Durability = value;
            }
            UpdateDurabilityText();
        }
    }
    private int _durability;

    public int MaxDurability
    {
        get
        {
            if(worldWeapon == null)
            {
                return weaponPrefab.GetComponent<Weapon>().MaxDurability;
            }
            else
            {
                return worldWeapon.MaxDurability;
            }
        }
    }
    private Weapon? worldWeapon;
    private bool stopSelfIntialize = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(weaponPrefab == null)
        {
            Debug.LogWarning("weapon prefab is null", this);
        }
        else if (!weaponPrefab.TryGetComponent<Weapon>(out _))
        {
            Debug.LogWarning("weapon prefabs lack Weapon script", this);
        }

        if (durabilityText == null) Debug.LogWarning("durabilityText is null");
    }
#endif

    private void Awake()
    {
        if (stopSelfIntialize) return;

        if (weaponPrefab.TryGetComponent(out Weapon component))
        {
            _durability = component.Durability;

            UpdateDurabilityText();
        }
    }

    public int GetRepairCost()
    {
        if (Durability >= MaxDurability) return 0;

        if (BlacksmithHelper.CostIsPerDurability)
        {
            return (MaxDurability - Durability) * BlacksmithHelper.Cost;
        }
        else
        {
            return BlacksmithHelper.Cost;
        }
    }

    public void StopSelfIntialize()
    {
        stopSelfIntialize = true;
    }

    public void Unequip()
    {
        if(worldWeapon != null)
        {
            worldWeapon.OnBreak.RemoveListener(Unequip);

            _durability = worldWeapon.Durability;
            worldWeapon = null;
        }
    }

    public GameObject GetEquipPrefab()
    {
        return weaponPrefab;
    }

    public void ConnectToWeapon(Weapon weaponScript)
    {
        worldWeapon = weaponScript;
        worldWeapon.OnBreak.AddListener(Unequip);
        worldWeapon.Durability = _durability;
        UpdateDurabilityText();
        worldWeapon.OnDamage.AddListener(UpdateDurabilityText);
    }

    private void UpdateDurabilityText()
    {
        durabilityText.text = Durability.ToString();
    }
}
