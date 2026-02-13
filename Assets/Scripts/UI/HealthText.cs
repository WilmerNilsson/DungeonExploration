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
        UpdateText(new HealthData(playerHealth.CurrentHealth, playerHealth.MaxHealth));
        playerHealth.OnChangeHealths.AddListener(UpdateText);
    }

    private void UpdateText(HealthData healthData)
    {
        myText.text = $"{healthData.CurrentHealth}/{healthData.MaxHealth}";
    }

    private void OnDestroy()
    {
        playerHealth.OnChangeHealths.RemoveListener(UpdateText);
    }
}
