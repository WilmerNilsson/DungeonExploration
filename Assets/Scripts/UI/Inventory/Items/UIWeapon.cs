using System;
using TMPro;
using UnityEngine;

public class UIWeapon : MonoBehaviour
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
                return _maxDurability;
            }
            else
            {
                return worldWeapon.MaxDurability;
            }
        }
        set
        {
            if (worldWeapon == null)
            {
                _maxDurability = value;
            }
            else
            {
                worldWeapon.MaxDurability = value;
            }
        }
    }
    private int _maxDurability;
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

    private void Start()
    {
        if (weaponPrefab.TryGetComponent(out Weapon component))
        {
            MaxDurability = component.MaxDurability;
            if (stopSelfIntialize) return;
            worldWeapon = component;
            Durability = component.Durability;
            UpdateDurabilityText();
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
            _durability = worldWeapon.Durability;
            _maxDurability = worldWeapon.MaxDurability;
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
        worldWeapon.Durability = _durability;
        worldWeapon.MaxDurability = _maxDurability;
        UpdateDurabilityText();
        worldWeapon.OnDamage.AddListener(UpdateDurabilityText);
    }

    private void UpdateDurabilityText()
    {
        durabilityText.text = Durability.ToString();
    }
}
