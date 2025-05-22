using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DinamicCanvasScale : MonoBehaviour
{
    public CanvasScaler[] scalers;
    public float minMatch = 0.3f;
    public float maxMatch = 0.8f;

    void Start()
    {
        foreach (CanvasScaler scaler in scalers)
        {
            AdjustCanvasScaler(scaler);
        }
    }

    void AdjustCanvasScaler(CanvasScaler canvas)
    {
        float screenAspect = (float)Screen.width / Screen.height;
        float referenceAspect = 1920f / 1080f;

        if (screenAspect > referenceAspect)
        {
            canvas.matchWidthOrHeight = minMatch; //Priorizar a la altura
        }
        else
        {
            canvas.matchWidthOrHeight = maxMatch; //Priorizar a la anchura
        }
    }

}
