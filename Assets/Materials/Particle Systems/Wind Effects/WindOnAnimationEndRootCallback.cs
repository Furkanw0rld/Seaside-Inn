using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindOnAnimationEndRootCallback : MonoBehaviour
{
    public void OnAnimationEnd()
    {
        Destroy(gameObject);
    }
}
