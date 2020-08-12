using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class HiddenMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject hiddenMenu;
    public TextMeshProUGUI vsyncText;
    public TextMeshProUGUI currentTimeMultiplierText;
    public TextMeshProUGUI currentEntityCountText;

    UniversalRenderPipelineAsset urp;
    [Header("NPC Spawner")]
    public GameObject npcToSpawn;

    private GameObject player; //Player
    private int entityCount = 9; // Starting characters in the scene.

    // Start is called before the first frame update
    void Start()
    {
        urp = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;

        if (QualitySettings.vSyncCount == 0)
        {
            vsyncText.text = "vSync disabled.";
        }
        else
        {
            vsyncText.text = "vSync enabled.";
        }
        player = PlayerManager.Instance.player;

        currentTimeMultiplierText.text = "Current Time Speed: " + DayNightCycle.Instance.ModifyTimeMultiplier(0) + "x";
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            hiddenMenu.gameObject.SetActive(!hiddenMenu.gameObject.activeSelf);
        }
    }

    public void OpenHiddenMenu()
    {
        hiddenMenu.gameObject.SetActive(!hiddenMenu.gameObject.activeSelf);
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
        Instantiate(npcToSpawn, player.transform.position + (player.transform.right * 2.5f), Quaternion.identity);
        entityCount++;
        currentEntityCountText.text = "There are: " + entityCount + " characters.";
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeShadowMode()
    {
        if(urp.shadowDistance > 0f)
        {
            urp.shadowDistance = 0f;
        }
        else
        {
            urp.shadowDistance = 100f;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
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

    public void AddCoins()
    {
        PlayerInventory.Instance.AddCoins(25f);
    }

    public void RemoveCoins()
    {
        if(PlayerInventory.Instance.GetCoins() < 25f)
        {
            return;
        }
        PlayerInventory.Instance.AddCoins(-25f);
    }
}
