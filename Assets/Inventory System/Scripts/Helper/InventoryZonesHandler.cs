using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryZonesHandler : MonoBehaviour
{
    public static InventoryZonesHandler Instance;

    public delegate void OnPlayerEnterKitchenArea();
    [Tooltip("Player has entered Kitchen Area (Inn Inventory)")] public OnPlayerEnterKitchenArea onPlayerEnterKitchenAreaCallback; // Player has entered the Kitchen Area

    public delegate void OnPlayerExitKitchenArea();
    [Tooltip("Player has exited Kitchen Area (Inn Inventory)")] public OnPlayerExitKitchenArea onPlayerExitKitchenAreaCallback; // Player has left the Kitchen Area

    public delegate void OnPlayerEnterTradeArea();
    [Tooltip("Player has entered Trade Area (Trade Inventory)")] public OnPlayerEnterTradeArea onPlayerEnterTradeAreaCallback;

    public delegate void OnPlayerExitTradeArea();
    [Tooltip("Player has exited Trade Area (Trade Inventory)")] public OnPlayerExitTradeArea onPlayerExitTradeAreaCallback;

    public bool IsPlayerAtInnInventoryArea { get; private set; }
    public bool IsPlayerAtTradeInventoryArea { get; private set; }

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
        onPlayerEnterKitchenAreaCallback += delegate { IsPlayerAtInnInventoryArea = true; };
        onPlayerExitKitchenAreaCallback += delegate { IsPlayerAtInnInventoryArea = false; };

        onPlayerEnterTradeAreaCallback += delegate { IsPlayerAtTradeInventoryArea = true; };
        onPlayerExitTradeAreaCallback += delegate { IsPlayerAtTradeInventoryArea = false; };
    }
}
