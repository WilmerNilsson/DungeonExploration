using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private RectTransform childPanel;
    [SerializeField] private RectTransform GrandChildPanel;
    private Vector2 defaultSize;
    private float X;
    private float Y;
    private void Awake()
    {
        childPanel.gameObject.SetActive(false);
        defaultSize = childPanel.sizeDelta;
    }

    private void OnEnable()
    {
        GameObject.FindGameObjectWithTag("Player").TryGetComponent(out HumanoidInteract interact);
        interact.OnSee.AddListener(OnSee);
        interact.OnUnSee.AddListener(OnUnSee);
    }

    private void OnSee()
    {
        childPanel.gameObject.SetActive(true);
    }

    private void OnUnSee()
    {
        childPanel.gameObject.SetActive(false);
    }

    public void SetImageSizeX(float x)
    {
        X = x;
        childPanel.sizeDelta =  new Vector2(X, Y);
        GrandChildPanel.sizeDelta =  new Vector2(X, Y);
    }
    
    public void SetImageSizeY(float y)
    {
        Y = y;
        childPanel.sizeDelta = new Vector2(X, Y);
        GrandChildPanel.sizeDelta =  new Vector2(X, Y);
    }

    public void ResetImageSize()
    {
        childPanel.sizeDelta = defaultSize;
        GrandChildPanel.sizeDelta = defaultSize;
    }
}
