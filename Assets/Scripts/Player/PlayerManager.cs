using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public static PlayerManager Instance;
    [Header("Player Game Object Components")]
    public GameObject player;
    public PlayerAIMotor playerMotor;
    public PlayerAnimator playerAnimator;
    public PlayerController playerController;

    [Header("Player UI Components")]
    public TextMeshProUGUI coinsText;
    public Image coinsFill;

    public bool IsPlayerAtInnInventoryArea { get; private set; }
    public bool IsPlayerAtTradeInventoryArea { get; private set; }

    public delegate void OnInteractablePlayerFocused(Transform lookTarget); // Player entered a interaction with NPC
    [Tooltip("Player has entered an interaction with NPC")] public OnInteractablePlayerFocused onInteractablePlayerFocusedCallback;

    public delegate void OnInteractablePlayerUnFocused(); // Player has left the focus of player
    [Tooltip("Player has exited an interaction with NPC")] public OnInteractablePlayerUnFocused onInteractablePlayerUnFocusedCallback;

    public delegate void OnPlayerEnterKitchenArea(); 
    [Tooltip("Player has entered Kitchen Area (Inn Inventory)")] public OnPlayerEnterKitchenArea onPlayerEnterKitchenAreaCallback; // Player has entered the Kitchen Area

    public delegate void OnPlayerExitKitchenArea();
    [Tooltip("Player has exited Kitchen Area (Inn Inventory)")] public OnPlayerExitKitchenArea onPlayerExitKitchenAreaCallback; // Player has left the Kitchen Area

    public delegate void OnPlayerEnterTradeArea();
    [Tooltip("Player has entered Trade Area (Trade Inventory)")] public OnPlayerEnterTradeArea onPlayerEnterTradeAreaCallback;

    public delegate void OnPlayerExitTradeArea();
    [Tooltip("Player has exited Trade Area (Trade Inventory)")] public OnPlayerExitTradeArea onPlayerExitTradeAreaCallback;
    
    private void Awake()
    {
        if(Instance == null)
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
        onPlayerEnterKitchenAreaCallback += delegate{ IsPlayerAtInnInventoryArea = true; };
        onPlayerExitKitchenAreaCallback += delegate { IsPlayerAtInnInventoryArea = false; };

        onPlayerEnterTradeAreaCallback += delegate { IsPlayerAtTradeInventoryArea = true; };
        onPlayerExitTradeAreaCallback += delegate { IsPlayerAtTradeInventoryArea = false; };
    }

}
