using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottonSetSelectedColor : MonoBehaviour
{
    //Code taken mostly from Unity Mechanics (youtube)
    [SerializeField] Button button;
    //Color iNormalColor = Color.white;
    //Color iHighlightedColor = Color.white;
    ColorBlock cb;
    
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
