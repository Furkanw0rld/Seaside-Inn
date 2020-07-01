using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [HideInInspector] public static PlayerInventory Instance;

    private float coins = 1;
    private readonly float maximumCoinFillAmount = 250;
    private PlayerManager manager;

    private void Awake()
    {
        if (Instance == null)
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
        manager = PlayerManager.Instance;
        manager.coinsText.text = coins.ToString();
        manager.coinsFill.fillAmount = coins / maximumCoinFillAmount;
    }

    public float GetCoins()
    {
        return coins;
    }

    public float AddCoins(float amount)
    {
        coins += amount;
        manager.coinsText.text = coins.ToString();
        manager.coinsFill.fillAmount = coins / maximumCoinFillAmount;
        return coins;
    }
}
