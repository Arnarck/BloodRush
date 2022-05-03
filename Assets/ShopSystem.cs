using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ShopSystem : MonoBehaviour
{
    Dictionary<SaveData.Powerup, int> _powerupPrice = new Dictionary<SaveData.Powerup, int>();

    [SerializeField] int maxPowerupLevel;
    [SerializeField] TextMeshProUGUI coins;
    [SerializeField] Skin[] skins;
    [Header("Display")]
    [SerializeField] PowerupDisplay[] powerupDisplay;
    
    [Serializable]
    class Skin
    {
        public SaveData.SkinName name;
        public int price;
        public TextMeshProUGUI priceDisplay;
        public GameObject buyScreen;
    }

    [Serializable]
    class PowerupDisplay
    {
        public SaveData.Powerup name;
        public TextMeshProUGUI priceDisplay;
        public TextMeshProUGUI levelDisplay;
    }

    void Start()
    {
        SaveData.CreateAllData();
        SetPowerupPrices();
        SetSkinPrices();
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
            SaveData.ResetSkinData(SaveData.SkinName.MetalDemiAdany);
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
        float rawPrice = (10 * Mathf.Pow(x, 2f) ) * 5f;
        int price = (int)rawPrice;

        // Formula is 10x^2 * 5
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
            UpdatePowerupPrice(powerupName);
        }
    }


    // ================== SKINS ==================


    void SetSkinPrices()
    {
        foreach (Skin skin in skins)
        {
            skin.priceDisplay.text = skin.price.ToString();
        }
    }

    public void BuySkin(int index)
    {
        SaveData.SkinName skinName = (SaveData.SkinName)index;
        Skin skin = GetSkin(skinName);
        int storedBloodAmount = SaveData.GetInventoryData(SaveData.PlayerInventory.BloodAmount);
        int buyPrice = skin.price;

        Debug.Log("Demi Adny purchased: " + SaveData.IsSkinPurchased(skinName));
        if (storedBloodAmount >= buyPrice && !SaveData.IsSkinPurchased(skinName))
        {
            SaveData.ModifyBloodAmount(-buyPrice);
            SaveData.PurchaseSkin(skinName);
            skin.buyScreen.SetActive(false);
            Debug.Log("Demi Adny purchased: " + SaveData.IsSkinPurchased(skinName));
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
