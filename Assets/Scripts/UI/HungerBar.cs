using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{
    private Image image;
    private SlicedFilledImage sFImage;
    private bool isNormalImage;
    [SerializeField] private PlayerHungerSO hungerSO;

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
        UpdateInfo();
    }

    private void OnEnable()
    {
#if DEBUG
        if (hungerSO == null)
        {
            return;
        }
#endif

        hungerSO.OnChangeHunger.AddListener(UpdateInfo);

        UpdateInfo();
    }

    private void OnDisable()
    {
#if DEBUG
        if (hungerSO == null)
        {
            return;
        }
#endif

        hungerSO.OnChangeHunger.RemoveListener(UpdateInfo);
    }
    
    void UpdateInfo()
    {
        float current = hungerSO.CurrentHunger;
        float max = hungerSO.MaxHunger;
        if(isNormalImage)
        {
            image.fillAmount = Mathf.Clamp((float) current / max, 0, 1);
        }
        else
        {
            sFImage.fillAmount = Mathf.Clamp((float) current / max, 0, 1);
        }
        //optionalText?.SetText(current + "/" + max);
    }
}
