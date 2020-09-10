using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrillEffects : MonoBehaviour
{

    private new Light light;
#pragma warning disable 0649
    [Header("Light Information")]
    [SerializeField] private float maximumIntensity = 2f;
    [SerializeField] private float maximumCookingIntensity = 10.5f;
    [Header("Grills")]
    [SerializeField] private GrillingArea grillingAreaLeft;
    [SerializeField] private GrillingArea grillingAreaRight;
    [Header("Roaster")]
    [SerializeField] private Transform roaster;
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
        Vector3 roasterRotation = new Vector3(30f, 0f, 0f);
        while (true)
        {
            if (isAtBaseIntensity)
            {
                light.intensity = Mathf.Clamp(Mathf.PerlinNoise(noiseX * Time.time, noiseY), 0, 1) * maximumIntensity;
            }
            else
            {
                light.intensity = Mathf.Clamp(Mathf.PerlinNoise(noiseX * Time.time, noiseY), 0, 1) * maximumCookingIntensity;
                roaster.Rotate(roasterRotation * Time.deltaTime);
            }
            
            yield return null;
        }
    }
}
