using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GInventory 
{
    List<GameObject> items = new List<GameObject>();

    public void AddItem(GameObject g)
    {
        items.Add(g);
    }
}
