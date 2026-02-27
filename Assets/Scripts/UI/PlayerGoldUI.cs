using TMPro;
using UnityEngine;

public class PlayerGoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private PlayerCashSO cashSO;

    private void OnEnable()
    {
        ChangeText(cashSO.CurrentCash);

        cashSO.OnCashChange += ChangeText;
    }

    private void OnDisable()
    {
        cashSO.OnCashChange -= ChangeText;
    }

    private void ChangeText(int newValue)
    {
        goldText.text = $"Gold - {newValue}";
    }
}
