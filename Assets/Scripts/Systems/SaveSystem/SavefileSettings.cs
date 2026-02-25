using System;
using UnityEngine;

[Serializable]
public class SavefileSettings
{
    public float NormalTimescale;

    public SavefileSettings()
    {
        NormalTimescale = 1.0f;
    }

    public SavefileSettings(float normalTimescale)
    {
        NormalTimescale = normalTimescale;
    }

    public SavefileSettings Clone()
    {
        return new(NormalTimescale);
    }
}
