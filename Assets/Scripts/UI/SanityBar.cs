using UnityEngine;
using UnityEngine.UI;

public class SanityBar : MonoBehaviour
{
    private Image image;
    private SlicedFilledImage sFImage;
    private bool isNormalImage;
    [SerializeField] private PlayerSanitySO sanitySO;

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
        if (sanitySO == null)
        {
            return;
        }
#endif

        sanitySO.OnChangeSanity.AddListener(UpdateInfo);

        UpdateInfo();
    }

    private void OnDisable()
    {
#if DEBUG
        if (sanitySO == null)
        {
            return;
        }
#endif

        sanitySO.OnChangeSanity.RemoveListener(UpdateInfo);
    }
    
    void UpdateInfo()
    {
        float current = sanitySO.CurrentSanity;
        float max = sanitySO.MaxSanity;
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
