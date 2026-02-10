using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    //[SerializeField] bool _isPlayerHealthBar = false; //we only have the player healtbar in this game

    [SerializeField] private TextMeshProUGUI optionalText;

    private Image image;
    private SlicedFilledImage sFImage;
    private bool isNormalImage;
    private Health health;

    private void Awake()
    {
        isNormalImage = TryGetComponent<Image>(out image);
        if(!isNormalImage)
        {
            sFImage = GetComponent<SlicedFilledImage>();
        }
    }

    private void Start()
    {
        health = GameObject.FindGameObjectWithTag("Player")?.GetComponentInChildren<Health>();

#if DEBUG
        if(health == null)
        {
            Debug.LogWarning("failed to find player health, disabling script", this);
            gameObject.SetActive(false);
            return;
        }
#endif

        UpdateInfo(health.CurrentHealth, health.MaxHealth);
    }

    private void OnEnable()
    {
#if DEBUG
        if (health == null)
        {
            return;
        }
#endif

        health.OnChangeHealths += UpdateInfo;

        UpdateInfo(health.CurrentHealth, health.MaxHealth);
    }

    private void OnDisable()
    {
#if DEBUG
        if (health == null)
        {
            return;
        }
#endif

        health.OnChangeHealths -= UpdateInfo;
    }

    void UpdateInfo(int current, int max)
    {
        if(isNormalImage)
        {
            image.fillAmount = Mathf.Clamp((float) current / max, 0, 1);
        }
        else
        {
            sFImage.fillAmount = Mathf.Clamp((float) current / max, 0, 1);
        }
        optionalText?.SetText(current + "/" + max);
    }
}
