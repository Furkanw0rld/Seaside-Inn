using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCGenerator : MonoBehaviour
{
    [Header("NPC")]
    public GameObject[] characters;
    public Material[] npcMaterials;
    public GameObject[] possibleItems;
    public TextMeshPro characterName;

    private string[] ladyNames = new string[32] {"Millicent", "Alianor", "Ellyn", "Jacquelyn", "Catherine", "Anne", "Thea", "Elizabeth", "Luella", "Mary", "Arabella", "Muriel", "Isolde", "Eleanor", "Josselyn", "Margaret", "Ariana", "Clarice",
    "Idla", "Claire", "Ryia", "Joan", "Clemence", "Morgaine", "Edith", "Nerida", "Ysmay", "Gwendolynn", "Victoria", "Rose", "Katelyn", "Juliana"};
    private string[] lordNames = new string[32] { "Baird", "Henry", "Oliver", "Fraden", "John", "Geoffrey", "Francis", "Frederick", "Thomas", "Cassius", "Richard", "Matthew", "Charles", "Favian", "Philip", "Zoricus", "Carac", "Sadon",
    "Alistair", "Caine", "Godfrey", "Mericus", "Rowley", "Brom", "Cornell", "Merek", "Alfred", "Cedric", "Gregory", "William", "Robert", "Bartholomew"};
    // Start is called before the first frame update
    void Start()
    {
        int index = Random.Range(0, characters.Length); //Pick Character
        characters[index].SetActive(true);
        SkinnedMeshRenderer renderer = characters[index].GetComponent<SkinnedMeshRenderer>(); // Cache renderer
        GenerateName(index);

        index = Random.Range(0, possibleItems.Length); //Pick a random item, can be empty
        if(possibleItems[index] != null)
        {
            possibleItems[index].SetActive(true);
        }

        index = Random.Range(0, npcMaterials.Length); //Pick a random outfit
        Material[] mats = renderer.materials;
        mats[1] = npcMaterials[index]; //Swap to outfit Note: mats[0] has flat shader, and mats[1] is outfit material.
        renderer.materials = mats; //Apply to renderer
    }

    private void GenerateName(int i)
    {
        if(i < 6) // 0 - 5 Female
        {
            characterName.text = ladyNames[Random.Range(0, ladyNames.Length)];
        }
        else // Male
        {
            characterName.text = lordNames[Random.Range(0, lordNames.Length)];
        }
    }
}
