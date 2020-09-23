using UnityEngine;

public class InnInventoryArea : MonoBehaviour
{
    private InventoryZonesHandler inventoryZones;

    private void Start()
    {
        if (InventoryZonesHandler.Instance)
        {
            inventoryZones = InventoryZonesHandler.Instance;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inventoryZones.onPlayerEnterKitchenAreaCallback?.Invoke();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inventoryZones.onPlayerExitKitchenAreaCallback?.Invoke();
        }

    }
}
