using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryKorsan))]
public class InteractableKorsan : Interactable
{
    private PlayerManager playerManager;
    private InventoryKorsan korsanInventory;

    // Start is called before the first frame update
    protected override void Start()
    {
        playerManager = PlayerManager.Instance;
        korsanInventory = GetComponent<InventoryKorsan>();
    }


    public override void Interact()
    {
        playerManager.onInteractablePlayerFocusedCallback?.Invoke(this.transform);
        base.Interact();
        korsanInventory.OpenShop();

    }

    public override void OnDeFocus()
    {
        playerManager.onInteractablePlayerUnFocusedCallback?.Invoke();
        base.OnDeFocus();
        korsanInventory.CloseShop();
    }
}
