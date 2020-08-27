using System.Collections;
using UnityEngine;
using TMPro;
using I2.Loc;
using UnityEngine.Events;

public class GameTimeManager : MonoBehaviour
{
#pragma warning disable 0649
	[Header("Light Sources")]
	[SerializeField] private GameObject sunSource;
	[SerializeField] private GameObject moonSource;
	[Header("Skyboxes")]
	[SerializeField] private Material dayTimeSkybox;
	[SerializeField] private Material nightTimeSkybox;
	[SerializeField] private TextMeshProUGUI dayText;
	[SerializeField] private TextMeshProUGUI timeText;
	[SerializeField] [Range(-2f, 2f)] private float skyboxRotationSpeed;
#pragma warning restore 0649

	private Transform sunTransform;
	private Transform moonTransform;

	private readonly float secondsPerDay = 1440f;
	private int currentDay = 1;
	private int currentHour = 0;
	private int currentMinute = 0;
	[Header("Time Controls")]
	[Range(0, 1440)] [Tooltip("6AM is 360. 8AM is 480. 12PM is 720. 6PM is 1080. 10PM is 1320. 11:59PM is 1439. Midnight is 0.")] [SerializeField] private float currentTime = 480f;
	[Tooltip("1X is 24 minutes per day.")] [SerializeField] private float timeMultiplier = 1f;
	[Header("UI Elements")]

	[Header("Localization")]
	public LocalizationParamsManager localDayParamManager;

 /* Midnight - 0f
 * 6AM - 360f
 * 8AM - 480f
 * 12PM - 720f
 * 6PM - 1080f
 * 10PM - 1320f
 * 11:59PM - 1439f
 * 
 * Rotation Daytime 6AM - 10PM -> X: 0F => 180F
 * Rotation Nighttime 10PM - 6AM -> 180F => 360F
 */
	public static GameTimeManager Instance;

	public delegate void OnNightTime();
	public OnNightTime onNightTimeCallback;

	public delegate void OnDayTime();
	public OnDayTime onDayTimeCallback;

	public int GetCurrentDay()
	{
		return currentDay;
	}

	public bool IsDayTime()
	{
		if (currentTime >= 360 && currentTime < 1320) //Adjust current day time
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public float GetTimeMultiplier()
	{
		return timeMultiplier;
	}

	public void SetTime(float t)
	{
		currentTime = t;
	}

	public float ModifyTimeMultiplier(float amount)
	{
		timeMultiplier += amount;
		if (timeMultiplier < 1f)
		{
			timeMultiplier = 1f;
		}
		return timeMultiplier;
	}

	public ulong GetWorldTime()
	{
		string time = "";
		time += currentDay + ((int)currentTime).ToString().PadLeft(4, '0');
		return ulong.Parse(time);
	}

	public IEnumerator WaitUntilWorldTime(ulong time, UnityAction callback)
    {
		ulong dayToWaitUntil = GetDayFromWorldTime(time);
		ushort timeToWaitUntil = GetTimeFromWorldTime(time);

		while(dayToWaitUntil > (uint)currentDay)
        {
			yield return new WaitForSeconds(5f);
        }

		while (timeToWaitUntil > currentTime)
        {
			yield return new WaitForSeconds(5f);
        }

		callback.Invoke();
	}

	public ulong GetNextWorldTime(ulong time, ushort timeToAdd)
	{
		ushort hour = GetTimeFromWorldTime(time);

		if(hour + timeToAdd >= secondsPerDay)
		{
			if(timeToAdd > secondsPerDay) //Adding more than a day
			{
				ushort addedDays = 0;
				ushort adding = timeToAdd;

				while(adding >= secondsPerDay)
				{
					adding -= (ushort)secondsPerDay;
					addedDays++;
				}

				ulong nextDay = GetDayFromWorldTime(time);
				nextDay += addedDays;
				string nextTime = "";

				if(hour + adding >= secondsPerDay) //We still need to go an extra day
				{
					ushort timeRemaining = (ushort)Mathf.Abs(secondsPerDay - hour);
					adding -= timeRemaining;
					nextDay++;
					nextTime += nextDay + adding.ToString().PadLeft(4, '0');
				}
				else // Add the hours to the new day
				{
					nextTime += nextDay + (hour + adding).ToString().PadLeft(4, '0');
				}

				return ulong.Parse(nextTime);
			}
			else if(timeToAdd == secondsPerDay) //Adding exactly a day
            {
                ulong nextDay = GetDayFromWorldTime(time);
				nextDay++;
				string nextTime = "";
				nextTime += nextDay + hour.ToString().PadLeft(4, '0');
				return ulong.Parse(nextTime);
			}
			else //Adding less than a day, but we are going past a day
			{
				ulong nextDay = GetDayFromWorldTime(time);
				ushort timeRemaining = (ushort)Mathf.Abs(secondsPerDay - hour);
				ushort timeAdding = timeToAdd;
				string nextTime = "";

				timeAdding -= timeRemaining;
				nextDay++;
				nextTime += nextDay + timeAdding.ToString().PadLeft(4, '0');
				return ulong.Parse(nextTime);

			}
		}
		else //Same day, increment hour
		{
			string nextTime = "";
			nextTime += GetDayFromWorldTime(time) + (hour + timeToAdd).ToString().PadLeft(4, '0');
			return ulong.Parse(nextTime);
		}
	}

	private ushort GetTimeFromWorldTime(ulong time)
	{
		return (ushort)(time % 10000);
	}

	private ulong GetDayFromWorldTime(ulong time)
	{
		for(int i = 0; i < 4; i++)
		{
			time /= 10;
		}
		return time;
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this);
		}
	}

	private void Start()
	{
		UpdateGameClock();
		sunTransform = sunSource.transform;
		moonTransform = moonSource.transform;
		StartCoroutine(TimeController());
	}

	private IEnumerator TimeController()
	{
		float timeNormalized;
		while (true)
		{
			currentTime += Time.deltaTime * timeMultiplier;

			if (currentTime >= secondsPerDay)
			{
				currentTime = 0f;
				currentDay++;
			}

			if (currentTime >= 360f && currentTime <= 1320f) // Time is between 6AM and 10PM (Day Time)
			{
				if (moonSource.activeSelf || !sunSource.activeSelf)
				{
					moonSource.SetActive(false);
					sunSource.SetActive(true);
					RenderSettings.skybox = dayTimeSkybox;
					DynamicGI.UpdateEnvironment();
					onDayTimeCallback?.Invoke();

				}

				timeNormalized = ((currentTime - 360f) / (1320f - 360f)) * 180f;
				sunTransform.rotation = Quaternion.Euler(timeNormalized, 130f, 90f);
			}
			else // Time is between 10PM and 6AM (Night Time)
			{
				if (sunSource.activeSelf || !moonSource.activeSelf)
				{
					sunSource.SetActive(false);
					moonSource.SetActive(true);
					RenderSettings.skybox = nightTimeSkybox;
					DynamicGI.UpdateEnvironment();
					onNightTimeCallback?.Invoke();
				}

				if (currentTime > 1320f) // 10PM and Midnight
				{
					timeNormalized = (((currentTime - 1320f) / (1440f - 1320f)) * 14);
					moonTransform.rotation = Quaternion.Euler(timeNormalized, 130f, 90f);
				}
				else
				{
					timeNormalized = ((currentTime / 360f) * (180 - 14)) + 14;
					moonTransform.rotation = Quaternion.Euler(timeNormalized, 130f, 90f);
				}
			}

			UpdateGameClock();
			yield return null;
		}

	}

	private void UpdateGameClock()
	{
		currentHour = Mathf.FloorToInt(currentTime / 60);
		currentMinute = Mathf.FloorToInt(currentTime - (currentHour * 60));
		timeText.text = currentHour.ToString().PadLeft(2, '0') + ":" + currentMinute.ToString().PadLeft(2, '0');
		//dayText.text = "Day " + currentDay.ToString().PadLeft(2, '0');
		localDayParamManager.SetParameterValue("CURRENT_DAY", currentDay.ToString().PadLeft(2, '0'));

		//Rotating skybox here, might need to move if realtime cycle is adjusted.
		RenderSettings.skybox.SetFloat("_Rotation", skyboxRotationSpeed * Time.time);
	}

}
