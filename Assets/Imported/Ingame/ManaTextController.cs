using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManaTextController : MonoBehaviour
{
    TextMeshProUGUI text;
    Mana playerMana;

    int maxMana;
    int currentMana;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        playerMana = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Mana>();
    }

    void OnEnable()
    {
        maxMana = playerMana.GetMaxMana();
        currentMana = playerMana.GetCurrentMana();
        UpdateText();

        playerMana.OnMaxManaChangeAction += SetMaxMana;
        playerMana.OnCurrentManaChangeAction += SetCurrentMana;
    }

    void OnDisable()
    {
        playerMana.OnMaxManaChangeAction -= SetMaxMana;
        playerMana.OnCurrentManaChangeAction -= SetCurrentMana;
    }

    void UpdateText()
    {
        text.SetText(currentMana.ToString() + "/" + maxMana.ToString());
    }

    void SetMaxMana(int newValue)
    {
        maxMana = newValue;
        UpdateText();
    }

    void SetCurrentMana(int newValue)
    {
        currentMana = newValue;
        UpdateText();
    }
}
