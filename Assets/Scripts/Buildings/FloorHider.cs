using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FloorHider : MonoBehaviour
{
    public GameObject floorToHide;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach(Transform child in floorToHide.transform)
            {
                child.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                child.gameObject.layer = 2;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (Transform child in floorToHide.transform)
            {
                child.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.On;
                child.gameObject.layer = 0;
            }
        }
    }
}
