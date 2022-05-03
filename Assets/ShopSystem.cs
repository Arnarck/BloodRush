using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ShopSystem : MonoBehaviour
{
    Dictionary<SaveData.Powerup, int> _powerupPrice = new Dictionary<SaveData.Powerup, int>();

    [SerializeField] int maxPowerupLevel;
    [SerializeField] Skin[] skins;
    [SerializeField] PowerupPriceDisplay[] powerupPriceDisplay;
    
    [Serializable]
    class Skin
    {
        public SaveData.SkinName name;
        public int price;
    }

    [Serializable]
    class PowerupPriceDisplay
    {
        public SaveData.Powerup name;
        public TextMeshProUGUI priceDisplay;
        public TextMeshProUGUI levelDisplay;
    }

    void Start()
    {
        SaveData.CreateAllData();
        CreatePowerupPrices();
    }

    void CreatePowerupPrices()
    {
        foreach (SaveData.Powerup powerupName in Enum.GetValues(typeof(SaveData.Powerup)))
        {
            _powerupPrice.Add(powerupName, GetPowerupPrice(powerupName));
            UpdatePowerupPrice(powerupName);
        }
    }


    public void BuyPowerupUpgrade(SaveData.Powerup powerupName)
    {
        int storedBloodAmount = SaveData.GetInventoryData(SaveData.PlayerInventory.BloodAmount);
        int upgradePrice = _powerupPrice[powerupName];

        if (SaveData.GetPowerupLevel(powerupName) >= maxPowerupLevel) return;

        if (storedBloodAmount >= upgradePrice)
        {
            SaveData.ModifyBloodAmount(storedBloodAmount - upgradePrice);
            SaveData.IncreasePowerupLevel(powerupName);
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
        PowerupPriceDisplay priceDisplayer = GetPowerupPriceDisplay(powerupName);

        priceDisplayer.priceDisplay.text = GetPowerupPrice(powerupName).ToString();
        priceDisplayer.levelDisplay.text = SaveData.GetPowerupLevel(powerupName).ToString();
    }

    int GetPowerupPrice(SaveData.Powerup name)
    {
        int powerupLevel = SaveData.GetPowerupLevel(name);

        // Formula is 10x^2 * 5
        return (10 * powerupLevel ^ 2) * 5;
    }

    PowerupPriceDisplay GetPowerupPriceDisplay(SaveData.Powerup name)
    {
        foreach (PowerupPriceDisplay priceDisplay in powerupPriceDisplay)
        {
            if (priceDisplay.name.Equals(name)) return priceDisplay;
        }

        Debug.LogError("Powerup Price Display " + name + " not found!");
        return null;
    }



    public void BuySkin(SaveData.SkinName skinName)
    {
        int storedBloodAmount = SaveData.GetInventoryData(SaveData.PlayerInventory.BloodAmount);
        int buyPrice = GetSkin(skinName).price;

        if (storedBloodAmount >= buyPrice && !SaveData.IsSkinPurchased(skinName))
        {
            SaveData.ModifyBloodAmount(storedBloodAmount - buyPrice);
            SaveData.PurchaseSkin(skinName);
        }
    }

    Skin GetSkin(SaveData.SkinName skinName)
    {
        foreach (Skin skin in skins)
        {
            if (skin.name.Equals(skinName)) return skin;
        }

        Debug.LogError("Skin " + skinName + " not found!");
        return null;
    }
}
