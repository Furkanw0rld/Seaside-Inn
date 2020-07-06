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

    [Header("Player UI Components")]
    public TextMeshProUGUI coinsText;
    public Image coinsFill;

    public delegate void OnInteractablePlayerFocused(Transform lookTarget);
    public OnInteractablePlayerFocused onInteractablePlayerFocusedCallback;

    public delegate void OnInteractablePlayerUnFocused();
    public OnInteractablePlayerUnFocused onInteractablePlayerUnFocusedCallback;
    
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
