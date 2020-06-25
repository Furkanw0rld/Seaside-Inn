using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public static PlayerManager Instance;

    public GameObject player;
    public PlayerAIMotor playerMotor;
    public PlayerAnimator playerAnimator;
    
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
