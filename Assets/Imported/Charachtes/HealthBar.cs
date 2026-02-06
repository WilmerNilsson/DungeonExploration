using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] bool _isPlayerHealthBar = false;
    [SerializeField] bool _isBossHealthBar = false;

    TextMeshProUGUI _text;

    Image _image;
    SlicedFilledImage _sFImage;
    bool _isNormalImage;
    Health _health;

    private static HealthBar s_bossHpBar;

    int _currentHP;
    int _maxHP;

    private void Awake()
    {
        _isNormalImage = TryGetComponent<Image>(out _image);
        if(!_isNormalImage)
        {
            _sFImage = GetComponent<SlicedFilledImage>();
        }
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        if(_isPlayerHealthBar)
        {
            _health = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Health>();

            //_health.OnCurrentHealthChangeAction += SetCurrentHP;
            //_health.OnMaxHealthChangeAction += SetMaxHP;

            //SetCurrentHP(_health.GetCurrentHealth());
            //SetMaxHP(_health.GetMaxHealth());
        }
        else if(_isBossHealthBar)
        {
            s_bossHpBar = this;
        }
    }

    private void OnDisable()
    {
        if(_health != null)
        {
            //_health.OnCurrentHealthChangeAction -= SetCurrentHP;
            //_health.OnMaxHealthChangeAction -= SetMaxHP;
        }
    }

    private void SetBossHpInstance_(Health bossHealth)
    {
        if(_health != null)
        {
            //_health.OnCurrentHealthChangeAction -= SetCurrentHP;
            //_health.OnMaxHealthChangeAction -= SetMaxHP;
        }
        
        _health = bossHealth;

        //_health.OnCurrentHealthChangeAction += SetCurrentHP;
        //_health.OnMaxHealthChangeAction += SetMaxHP;

        //SetCurrentHP(_health.GetCurrentHealth());
        //SetMaxHP(_health.GetMaxHealth());
    }

    void UpdateInfo()
    {
        if(_isNormalImage)
        {
            _image.fillAmount = Mathf.Clamp((float) _currentHP / _maxHP, 0, 1);
        }
        else
        {
            _sFImage.fillAmount = Mathf.Clamp((float) _currentHP / _maxHP, 0, 1);
        }
        _text.SetText(_currentHP + "/" + _maxHP);
    }

    void SetCurrentHP(int newValue)
    {
        _currentHP = newValue;
        UpdateInfo();
    }

    void SetMaxHP(int newValue)
    {
        _maxHP = newValue;
        UpdateInfo();
    }
}
