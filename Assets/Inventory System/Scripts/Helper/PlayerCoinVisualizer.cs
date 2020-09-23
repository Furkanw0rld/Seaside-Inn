using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCoinVisualizer : MonoBehaviour
{

    [Header("Player UI Components")]
    public TextMeshProUGUI coinsText;
    public Image coinsFill;
    public float maximumCoinFillAmount = 250f;

    public void UpdateVisuals(float coins)
    {
        coinsText.text = string.Format("{0:0.##}", coins);
        coinsFill.fillAmount = coins / maximumCoinFillAmount;
    }
}
