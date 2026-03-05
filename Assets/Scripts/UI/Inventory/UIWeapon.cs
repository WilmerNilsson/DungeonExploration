using UnityEngine;

public class UIWeapon : MonoBehaviour
{
    [SerializeField] private GameObject weaponPrefab;

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
    }
#endif

    public void Unequip()
    {

    }

    /// <summary>
    /// returns the prefab
    /// </summary>
    /// <returns></returns>
    public GameObject GetEquipPrefab()
    {
        return weaponPrefab;
    }
}
