using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    Image image;
    Mana playerMana;

    int maxMana;
    int currentMana;

    private void Awake()
    {
        image = GetComponent<Image>();
        playerMana = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Mana>();
    }

    void OnEnable()
    {
        maxMana = playerMana.GetMaxMana();
        currentMana = playerMana.GetCurrentMana();
        UpdateBar();

        playerMana.OnMaxManaChangeAction += SetMaxMana;
        playerMana.OnCurrentManaChangeAction += SetCurrentMana;
    }

    void OnDisable()
    {
        playerMana.OnMaxManaChangeAction -= SetMaxMana;
        playerMana.OnCurrentManaChangeAction -= SetCurrentMana;
    }

    void UpdateBar()
    {
        image.fillAmount = Mathf.Clamp((float) currentMana / maxMana, 0, 1);
    }

    void SetMaxMana(int newValue)
    {
        maxMana = newValue;
        UpdateBar();
    }

    void SetCurrentMana(int newValue)
    {
        currentMana = newValue;
        UpdateBar();
    }
}
