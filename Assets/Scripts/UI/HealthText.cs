using System;
using TMPro;
using UnityEngine;

public class HealthText : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    private TextMeshProUGUI myText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myText = GetComponent<TextMeshProUGUI>();
        UpdateText(playerHealth.GetHealth(), playerHealth.GetMaxHealth());
        playerHealth.OnChangeHealths += UpdateText;
    }

    private void UpdateText(int current, int max)
    {
        myText.text = $"{current}/{max}";
    }

    private void OnDestroy()
    {
        playerHealth.OnChangeHealths -= UpdateText;
    }
}
