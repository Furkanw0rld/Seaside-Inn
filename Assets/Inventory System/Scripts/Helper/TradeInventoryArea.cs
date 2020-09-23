using UnityEngine;

public class TradeInventoryArea : MonoBehaviour
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
            inventoryZones.onPlayerEnterTradeAreaCallback?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inventoryZones.onPlayerExitTradeAreaCallback?.Invoke();
        }
    }
}
