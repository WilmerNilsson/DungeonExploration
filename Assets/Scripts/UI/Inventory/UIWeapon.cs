using System;
using TMPro;
using UnityEngine;

public class UIWeapon : MonoBehaviour
{
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private TextMeshProUGUI durabilityText;

#nullable enable
    private int Durability
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
            if (worldWeapon != null) Debug.LogWarning("can't change durability of world weapon", this);
            _durability = value; 
        }
    }
    private int _durability;
    private Weapon? worldWeapon;

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
        if(weaponPrefab.TryGetComponent(out Weapon component))
        {
            Durability = component.Durability;
            UpdateDurabilityText();
        }
    }

    public void Unequip()
    {
        worldWeapon = null;
    }

    public GameObject GetEquipPrefab()
    {
        return weaponPrefab;
    }

    public void ConnectToWeapon(Weapon weaponScript)
    {
        worldWeapon = weaponScript;
        if(worldWeapon.Durability >  _durability)
        {
            int diff = worldWeapon.Durability - _durability;

            worldWeapon.LoseDurability(diff);
        }
        UpdateDurabilityText();
        worldWeapon.OnDamage.AddListener(UpdateDurabilityText);
    }

    private void UpdateDurabilityText()
    {
        durabilityText.text = Durability.ToString();
    }
}
