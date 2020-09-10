using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class NightLight : MonoBehaviour
{
	[Tooltip("Can be left null.")] public GameObject lightBeam;
	private Light currentLight;
	private GameTimeManager gameTimeManager;
	private float originalIntensity;

	private float noiseX = 0.0f;
	private float noiseY = 0.0f;
	// Start is called before the first frame update
	void Start()
	{
		currentLight = this.GetComponent<Light>();
		originalIntensity = currentLight.intensity;
		gameTimeManager = GameTimeManager.Instance; // Cache the Cycle

		if (gameTimeManager.IsDayTime()) //Make sure the light is set correctly without transition
		{
			currentLight.enabled = false;
			if (lightBeam)
			{
				lightBeam.SetActive(false);
			}
		}

		gameTimeManager.onNightTimeCallback += ChangeLightStateWrapper;
		gameTimeManager.onDayTimeCallback += ChangeLightStateWrapper;
		
	}

	private void OnDisable()
	{
		gameTimeManager.onDayTimeCallback -= ChangeLightStateWrapper;
		gameTimeManager.onNightTimeCallback -= ChangeLightStateWrapper;
	}

	//Wrapper Function to Invoke Light State Changer
	private void ChangeLightStateWrapper()
	{
		StartCoroutine(ChangeLightState());
	}

	private IEnumerator ChangeLightState()
	{
		yield return new WaitForSeconds(Random.Range( 1f / gameTimeManager.GetTimeMultiplier(), 10f / gameTimeManager.GetTimeMultiplier() )); //Delay Lights becoming active at same time

		if (gameTimeManager.IsDayTime() && currentLight.enabled)//Check if Daytime and if light is enabled
		{
			StartCoroutine(TransitionLight(currentLight.intensity, 0f, false));
		}
		else if (!currentLight.enabled)//Night Time and light is not enabled
		{
			StartCoroutine(TransitionLight(0f, originalIntensity, true));
		}
	}

	IEnumerator TransitionLight(float startIntensity, float desiredIntensity, bool desiredLightState)
	{
		float transitionTime = Random.Range(1f, 2.5f);

		if (desiredLightState) //If we want to turn the light on, turn it on prior to transition
		{
			GenerateFlickerSeed(); //Create a basic random seed so that light flickers differently
			currentLight.enabled = true;
			if (lightBeam)
			{
				lightBeam.SetActive(true);
			}
			desiredIntensity = GetPerlinNoise(transitionTime); //Get the future intensity position then transition to that.
			
		}

		for(float t = 0; t <= transitionTime; t+= Time.deltaTime)
		{
			currentLight.intensity = Mathf.Lerp(startIntensity, desiredIntensity, t / transitionTime);
			yield return null;
		}

		currentLight.intensity = desiredIntensity;

		if (!desiredLightState) //Turn the light off after transition
		{
			currentLight.enabled = false;
			if (lightBeam)
			{
				lightBeam.SetActive(false);
			}
		}
		else 
		{
			StartCoroutine(NightLightFlicker());
		}

	}

	private void GenerateFlickerSeed()
	{
		noiseX = Random.Range(0.1f, 0.3f); //Speed
		noiseY = Random.Range(0f, 10f); //Seed
	}

	private float GetPerlinNoise(float lookAhead) //Pass 0 to get current position or pass in a value to see that many seconds ahead.
	{
		return Mathf.Clamp(originalIntensity * Mathf.PerlinNoise((Time.time + lookAhead) * noiseX, noiseY), 10f, originalIntensity);
	}

	IEnumerator NightLightFlicker()
	{
		while (!gameTimeManager.IsDayTime()) //Flicker during Night Time
		{
			currentLight.intensity = GetPerlinNoise(0f); 
			yield return null;
		}
	}
}
