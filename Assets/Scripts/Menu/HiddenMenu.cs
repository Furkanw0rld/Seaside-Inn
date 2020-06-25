using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HiddenMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject hiddenMenu;
    public TextMeshProUGUI vsyncText;
    public TextMeshProUGUI currentTimeMultiplierText;
    public TextMeshProUGUI currentEntityCountText;
    [Header("NPC Spawner")]
    public GameObject npcToSpawn;

    private GameObject _player; //Player
    private int entityCount = 9;

    // Start is called before the first frame update
    void Start()
    {
        if(QualitySettings.vSyncCount == 0)
        {
            vsyncText.text = "vSync disabled.";
        }
        else
        {
            vsyncText.text = "vSync enabled.";
        }
        _player = GameObject.FindGameObjectWithTag("Player");

        currentTimeMultiplierText.text = "Current Time Speed: " + DayNightCycle.Instance.ModifyTimeMultiplier(0) + "x";
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (hiddenMenu.gameObject.activeSelf)
            {
                hiddenMenu.gameObject.SetActive(false);
            }
            else
            {
                hiddenMenu.gameObject.SetActive(true);
            }
        }
    }

    public void VSyncSetting()
    {
        if(QualitySettings.vSyncCount == 0) //No Vsync
        {
            QualitySettings.vSyncCount = 1;
            vsyncText.text = "vSync enabled.";
        }
        else //Vsync enabled
        {
            QualitySettings.vSyncCount = 0;
            vsyncText.text = "vSync disabled.";
        }
    }

    public void SpawnNPC()
    {
        Instantiate(npcToSpawn, _player.transform.position + (_player.transform.right * 2.5f), Quaternion.identity);
        entityCount++;
        currentEntityCountText.text = "There are: " + entityCount + " characters.";
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetTimeToMidnight()
    {
        DayNightCycle.Instance.SetTime(1440f);
    }

    public void SetTimeToNoon()
    {
        DayNightCycle.Instance.SetTime(720f);
    }

    public void SpeedUpTime()
    {
        currentTimeMultiplierText.text = "Current Time Speed: " + DayNightCycle.Instance.ModifyTimeMultiplier(1f) + "x";
    }

    public void SlowDownTime()
    {
        currentTimeMultiplierText.text = "Current Time Speed: " + DayNightCycle.Instance.ModifyTimeMultiplier(-1f) + "x";
    }
}
