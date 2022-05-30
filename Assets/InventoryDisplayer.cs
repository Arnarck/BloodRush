using UnityEngine;
using TMPro;

public class InventoryDisplayer : MonoBehaviour
{
    public static InventoryDisplayer Instance { get; private set; }

    [SerializeField] TextMeshProUGUI bloodCollected;
    [SerializeField] TextMeshProUGUI highScore;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        bloodCollected.text = SaveData.GetInventoryData(SaveData.PlayerInventory.BloodAmount).ToString();
        highScore.text = SaveData.GetInventoryData(SaveData.PlayerInventory.Highscore).ToString();
    }

    public void UpdateBloodAmount()
    {
        bloodCollected.text = SaveData.GetInventoryData(SaveData.PlayerInventory.BloodAmount).ToString();
    }
}
