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
            playerManager.onPlayerEnterTradeAreaCallback?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerManager.onPlayerExitTradeAreaCallback?.Invoke();
        }
    }
}
