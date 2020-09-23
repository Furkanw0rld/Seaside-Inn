using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    [Header("Player Game Object Components")]
    public GameObject player;
    public PlayerAIMotor playerMotor;
    public PlayerAnimator playerAnimator;
    public PlayerController playerController;

    public bool IsPlayerAtInnInventoryArea { get; private set; }
    public bool IsPlayerAtTradeInventoryArea { get; private set; }

    public delegate void OnInteractablePlayerFocused(Transform lookTarget); // Player entered a interaction with NPC
    [Tooltip("Player has entered an interaction with NPC")] public OnInteractablePlayerFocused onInteractablePlayerFocusedCallback;

    public delegate void OnInteractablePlayerUnFocused(); // Player has left the focus of player
    [Tooltip("Player has exited an interaction with NPC")] public OnInteractablePlayerUnFocused onInteractablePlayerUnFocusedCallback;

    
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

}
