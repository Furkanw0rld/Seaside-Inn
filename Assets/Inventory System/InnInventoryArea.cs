using UnityEngine;
using UnityEngine.Rendering;

public class InnInventoryArea : MonoBehaviour
{
    public GameObject kitchenRoof;

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
            playerManager.isPlayerAtInnInventoryArea = true;
            kitchenRoof.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            kitchenRoof.gameObject.layer = 2;

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerManager.isPlayerAtInnInventoryArea = false;
            kitchenRoof.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.On;
            kitchenRoof.gameObject.layer = 0;

        }

    }
}
