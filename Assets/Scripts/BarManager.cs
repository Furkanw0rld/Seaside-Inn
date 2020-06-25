using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarManager : MonoBehaviour
{
    [HideInInspector] public static BarManager Instance;
#pragma warning disable 0649
    [SerializeField] private GameObject[] barChairs;
#pragma warning restore 0649
    private InteractableBarChair[] barInteractables;
    
    private int emptyChairs;
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
        emptyChairs = barChairs.Length;
        barInteractables = new InteractableBarChair[barChairs.Length];

        for(int i = 0; i < barChairs.Length; i++)
        {
            barInteractables[i] = barChairs[i].GetComponent<InteractableBarChair>();
        }

    }

    public bool IsChairsAvailable()
    {
        return emptyChairs > 0;
    }

    private int PickChair()
    {
        if (IsChairsAvailable())
        {
            int temp = Random.Range(0, barChairs.Length);

            while (barInteractables[temp].IsOccupied)
            {
                temp = Random.Range(0, barChairs.Length);
            }

            return temp;
        }

        return -1;
    }

    public Interactable GetChair()
    {
        int pickedChair = PickChair();
        return barInteractables[pickedChair];
    }

}
