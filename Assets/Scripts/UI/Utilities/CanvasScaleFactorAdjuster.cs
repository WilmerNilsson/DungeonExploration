using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CanvasScaleFactorAdjuster : MonoBehaviour
{
    private CanvasScaler canvasScaler;
 
    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();

        AdjustScalingFactorFromInt(Screen.height / 360);
    }

    public void AdjustScalingFactorFromInt(int value)
    {
        canvasScaler.scaleFactor = value;
    }
}
