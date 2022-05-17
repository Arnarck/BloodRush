using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class PowerupUpgradeMenu : MonoBehaviour
{
    Dictionary<SaveData.Powerup, int> _powerupPrice = new Dictionary<SaveData.Powerup, int>();

    [SerializeField] int maxPowerupLevel;
    [SerializeField] TextMeshProUGUI coins;
    [Header("Display")]
    [SerializeField] PowerupDisplay[] powerupDisplay;

    [Serializable]
    class PowerupDisplay
    {
        public SaveData.Powerup name;
        public TextMeshProUGUI priceDisplay;
        public TextMeshProUGUI levelDisplay;
        public Button buyButton;
    }

    void Awake()
    {
        SaveData.CreateAllData();
    }

    void Start()
    {
        SetPowerupPrices();
    }

    void Update()
    {
        coins.text = SaveData.GetInventoryData(SaveData.PlayerInventory.BloodAmount).ToString();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveData.ModifyBloodAmount(1000);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SaveData.ResetInventoryData(SaveData.PlayerInventory.BloodAmount);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SaveData.ResetAllPowerupData();
        }
    }


    // ================== POWERUPS ==================


    void SetPowerupPrices()
    {
        foreach (SaveData.Powerup powerupName in Enum.GetValues(typeof(SaveData.Powerup)))
        {
            _powerupPrice.Add(powerupName, GetPowerupPrice(powerupName));
            UpdatePowerupPrice(powerupName);
        }
    }

    void UpdatePowerupPrice(SaveData.Powerup powerupName)
    {
        if (!_powerupPrice.ContainsKey(powerupName))
        {
            Debug.LogError("_powerupPrice does not contain an key for " + powerupName);
            return;
        }

        _powerupPrice[powerupName] = GetPowerupPrice(powerupName);
        PowerupDisplay priceDisplayer = GetPowerupPriceDisplay(powerupName);

        if (priceDisplayer != null)
        {
            priceDisplayer.priceDisplay.text = GetPowerupPrice(powerupName).ToString();
            priceDisplayer.levelDisplay.text = SaveData.GetPowerupLevel(powerupName).ToString();
        }
    }

    int GetPowerupPrice(SaveData.Powerup name)
    {
        float x = (float)SaveData.GetPowerupLevel(name);
        float rawPrice = (10 * Mathf.Pow(x, 3f) ) + 40f;
        int price = (int)rawPrice;

        // Formula is 10x^3 + 40
        return price;
    }

    PowerupDisplay GetPowerupPriceDisplay(SaveData.Powerup name)
    {
        foreach (PowerupDisplay priceDisplay in powerupDisplay)
        {
            if (priceDisplay.name.Equals(name)) return priceDisplay;
        }

        Debug.LogError("Powerup Price Display " + name + " not found!");
        return null;
    }

    public void BuyPowerupUpgrade(int index)
    {
        SaveData.Powerup powerupName = (SaveData.Powerup)index;
        int storedBloodAmount = SaveData.GetInventoryData(SaveData.PlayerInventory.BloodAmount);
        int upgradePrice = _powerupPrice[powerupName];

        if (SaveData.GetPowerupLevel(powerupName) >= maxPowerupLevel) return;

        if (storedBloodAmount >= upgradePrice)
        {
            SaveData.ModifyBloodAmount(-upgradePrice);
            SaveData.IncreasePowerupLevel(powerupName);

            if (SaveData.GetPowerupLevel(powerupName) >= maxPowerupLevel)
            {
                powerupDisplay[index].buyButton.interactable = false;
                powerupDisplay[index].buyButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Sold out";
            }
            else
            {
                UpdatePowerupPrice(powerupName);
            }
        }
    }
}
