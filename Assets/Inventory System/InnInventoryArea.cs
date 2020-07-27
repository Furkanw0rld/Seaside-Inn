using UnityEngine;

public class InnInventoryArea : MonoBehaviour
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
            playerManager.onPlayerEnterKitchenAreaCallback?.Invoke();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerManager.onPlayerExitKitchenAreaCallback?.Invoke();
        }

    }
}
