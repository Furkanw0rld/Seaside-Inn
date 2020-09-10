using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGrillLightFlicker : MonoBehaviour
{

    private new Light light;
#pragma warning disable 0649
    [SerializeField] private float maximumIntensity = 2f;
    [SerializeField] private float maximumCookingIntensity = 10.5f;
    [SerializeField] private GrillingArea grillingAreaLeft;
    [SerializeField] private GrillingArea grillingAreaRight;
#pragma warning restore 0649
    private const float noiseX = 0.25f; //Speed
    private const float noiseY = 5f; //Seed

    private bool isAtBaseIntensity = true;

    void Awake()
    {
        light = GetComponent<Light>();
        StartCoroutine(IntensityFlicker());
    }

    public void UpdateIntensity()
    {
        if (grillingAreaLeft.IsCooking || grillingAreaLeft.IsRecipeHere || grillingAreaRight.IsCooking || grillingAreaRight.IsRecipeHere)
        {
            isAtBaseIntensity = false;
        }
        else
        {
            isAtBaseIntensity = true;
        }
    }

    IEnumerator IntensityFlicker()
    {
        while (true)
        {
            if (isAtBaseIntensity)
            {
                light.intensity = Mathf.Clamp(Mathf.PerlinNoise(noiseX * Time.time, noiseY), 0, 1) * maximumIntensity;
            }
            else
            {
                light.intensity = Mathf.Clamp(Mathf.PerlinNoise(noiseX * Time.time, noiseY), 0, 1) * maximumCookingIntensity;
            }
            
            yield return null;
        }
    }
}
