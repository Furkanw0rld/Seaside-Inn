using UnityEngine;

public class TradeInventoryArea : MonoBehaviour
{
    private PlayerManager playerManager;

    private void Start()
    {
        if (PlayerManager.Instance)
        {
            playerManager = PlayerManager.Instance;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerManager.isPlayerAtTradeInventoryArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerManager.isPlayerAtTradeInventoryArea = false;
        }
    }
}
