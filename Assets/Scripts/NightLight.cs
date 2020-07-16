using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class NightLight : MonoBehaviour
{
	[Tooltip("Can be left null.")] public GameObject lightBeam;
	private Light _light;
	private DayNightCycle _cycle;
	private float _originalIntensity;

	private float _noiseX = 0.0f;
	private float _noiseY = 0.0f;
	// Start is called before the first frame update
	void Start()
	{
		_light = this.GetComponent<Light>();
		_originalIntensity = _light.intensity;
		_cycle = DayNightCycle.Instance; // Cache the Cycle

		if (_cycle.IsDayTime()) //Make sure the light is set correctly without transition
		{
			_light.enabled = false;
			if (lightBeam)
			{
				lightBeam.SetActive(false);
			}
		}

		_cycle.onNightTimeCallback += ChangeLightStateWrapper;
		_cycle.onDayTimeCallback += ChangeLightStateWrapper;
		
	}

	private void OnDisable()
	{
		_cycle.onDayTimeCallback -= ChangeLightStateWrapper;
		_cycle.onNightTimeCallback -= ChangeLightStateWrapper;
	}

	//Wrapper Function to Invoke Light State Changer
	private void ChangeLightStateWrapper()
	{
		StartCoroutine(ChangeLightState());
	}

	private IEnumerator ChangeLightState()
	{
		yield return new WaitForSeconds(Random.Range( 1f/_cycle.GetTimeMultiplier(), 10f / _cycle.GetTimeMultiplier() )); //Delay Lights becoming active at same time

		if (_cycle.IsDayTime() && _light.enabled)//Check if Daytime and if light is enabled
		{
			StartCoroutine(TransitionLight(_light.intensity, 0f, false));
		}
		else if (!_light.enabled)//Night Time and light is not enabled
		{
			StartCoroutine(TransitionLight(0f, _originalIntensity, true));
		}
	}



	IEnumerator TransitionLight(float startIntensity, float desiredIntensity, bool desiredLightState)
	{
		float transitionTime = Random.Range(1f, 2.5f);

		if (desiredLightState) //If we want to turn the light on, turn it on prior to transition
		{
			GenerateFlickerSeed(); //Create a basic random seed so that light flickers differently
			_light.enabled = true;
			if (lightBeam)
			{
				lightBeam.SetActive(true);
			}
			desiredIntensity = GetPerlinNoise(transitionTime); //Get the future intensity position then transition to that.
			
		}

		for(float t = 0; t <= transitionTime; t+= Time.deltaTime)
		{
			_light.intensity = Mathf.Lerp(startIntensity, desiredIntensity, t / transitionTime);
			yield return null;
		}

		_light.intensity = desiredIntensity;

		if (!desiredLightState) //Turn the light off after transition
		{
			_light.enabled = false;
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
		_noiseX = Random.Range(0.1f, 0.3f); //Speed
		_noiseY = Random.Range(0f, 10f); //Seed
	}

	private float GetPerlinNoise(float lookAhead) //Pass 0 to get current position or pass in a value to see that many seconds ahead.
	{
		return Mathf.Clamp(_originalIntensity * Mathf.PerlinNoise((Time.time + lookAhead) * _noiseX, _noiseY), 10f, _originalIntensity);
	}

	IEnumerator NightLightFlicker()
	{
		while (!_cycle.IsDayTime()) //Flicker during Night Time
		{
			_light.intensity = GetPerlinNoise(0f); 
			yield return null;
		}
	}
}
