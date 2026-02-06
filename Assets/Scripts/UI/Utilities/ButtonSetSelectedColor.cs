using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSetSelectedColor : MonoBehaviour
{
    //Code taken mostly from Unity Mechanics (youtube)
    [SerializeField] private Button button;
    //Color iNormalColor = Color.white;
    //Color iHighlightedColor = Color.white;
    private ColorBlock cb;

#if DEBUG
    private void OnValidate()
    {
        if(button == null)
        {
            Debug.LogWarning("button not set", this);
        }
    }
#endif

    void Start()
    {
        cb = button.colors;
        //iNormalColor = button.colors.normalColor;
        //HighlightedColor = button.colors.highlightedColor;
    }

    public void ChangeWhenHover()
    {
        cb.selectedColor = cb.highlightedColor;
        button.colors = cb;
    }

    public void ChangeWhenLeaves()
    {
        cb.selectedColor = cb.normalColor;
        button.colors = cb;
    }
}
