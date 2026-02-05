using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CanvasScaleFactorAdjuster : MonoBehaviour
{
    [SerializeField] bool hasCameraInScene = true;
    //UnityEngine.Rendering.Universal.PixelPerfectCamera ppc;
    CanvasScaler canvasScaler;
 
    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();

        if(hasCameraInScene)
        {
            //ppc = Camera.main.gameObject.GetComponent<UnityEngine.Rendering.Universal.PixelPerfectCamera>();
            AdjustScalingFactorAuto();
        }
        else
        {
            AdjustScalingFactorFromInt(Screen.height / 360);
        }
    }
 
    void AdjustScalingFactorAuto()
    {
        //canvasScaler.scaleFactor = ppc.pixelRatio;
    }

    public void AdjustScalingFactorFromInt(int value)
    {
        canvasScaler.scaleFactor = value;
    }
}
