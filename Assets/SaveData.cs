using UnityEngine;
using System;

public static class SaveData
{
    static int _powerupStartLevel = 1;
    static int _inventoryItemStartAmount = 0;

    public static int PowerupStartLevel { get => _powerupStartLevel; }
    public static int InventoryItemStartAmount { get => _inventoryItemStartAmount; }

    public enum Powerup
    {
        Fly,
        HigherJump,
        ScoreMultiplier,
    }

    public enum SkinName
    {
        MetalDemiAdany
    }

    public enum PlayerInventory
    {
        SelectedSkin,
        BloodAmount,
        Highscore
    }


    // =============== DATA CREATION ===============


    public static void CreateAllData()
    {
        CreateSkinData();
        CreatePowerupData();
        CreateInventoryData();
    }

    static void CreateSkinData()
    {
        foreach (SkinName skinName in Enum.GetValues(typeof(SkinName)))
        {
            string skin = skinName.ToString();
            if (!PlayerPrefs.HasKey(skin))
            {
                // Zero means "not purchased"
                PlayerPrefs.SetInt(skin, 0);
            }
        }
    }

    static void CreatePowerupData()
    {
        foreach (Powerup powerupName in Enum.GetValues(typeof(Powerup)))
        {
            string powerup = powerupName.ToString();
            if (!PlayerPrefs.HasKey(powerup))
            {
                PlayerPrefs.SetInt(powerup, PowerupStartLevel);
            }
        }
    }

    static void CreateInventoryData()
    {
        foreach (PlayerInventory playerInventory in Enum.GetValues(typeof(PlayerInventory)))
        {
            string inventory = playerInventory.ToString();
            if (!PlayerPrefs.HasKey(inventory))
            {
                PlayerPrefs.SetInt(inventory, InventoryItemStartAmount);
            }
        }
    }


    // =============== DATA MODIFICATION ===============


    public static void IncreasePowerupLevel(Powerup powerup)
    {
        string name = powerup.ToString();
        int currentLevel = PlayerPrefs.GetInt(name);
        int nextLevel = currentLevel + 1;

        PlayerPrefs.SetInt(name, nextLevel);
    }

    public static void PurchaseSkin(SkinName skin)
    {
        string name = skin.ToString();

        if (IsSkinPurchased(skin)) return;

        PlayerPrefs.SetInt(name, 1);
    }

    public static void SetHighScore(int score)
    {
        string name = PlayerInventory.Highscore.ToString();
        PlayerPrefs.SetInt(name, score);
    }

    public static void ModifyBloodAmount(int amount)
    {
        string name = PlayerInventory.BloodAmount.ToString();
        int currentAmount = PlayerPrefs.GetInt(name);
        int totalAmount = currentAmount + amount;

        totalAmount = Mathf.Clamp(totalAmount, 0, int.MaxValue);
        PlayerPrefs.SetInt(name, totalAmount);
    }

    public static void SetSelectedSkin(int index)
    {
        string name = PlayerInventory.SelectedSkin.ToString();
        PlayerPrefs.SetInt(name, index);
    }


    // =============== TOOLS ===============


    public static int GetSkinSize()
    {
        int count = 0;
        foreach (SkinName skin in Enum.GetValues(typeof(SkinName)))
        {
            count++;
        }

        return count;
    }

    public static bool IsSkinPurchased(SkinName skin)
    {
        string name = skin.ToString();
        int value = PlayerPrefs.GetInt(name);

        if (value == 0) return false;
        else return true;
    }

    public static int GetPowerupLevel(Powerup powerupName)
    {
        string name = powerupName.ToString();
        return PlayerPrefs.GetInt(name);
    }

    public static int GetInventoryData(PlayerInventory dataName)
    {
        string name = dataName.ToString();
        return PlayerPrefs.GetInt(name);
    }


    // =============== DEBUG ===============


    public static void ResetPowerupData(Powerup powerupName)
    {
        string powerup = powerupName.ToString();
        if (PlayerPrefs.HasKey(powerup))
        {
            PlayerPrefs.SetInt(powerup, _powerupStartLevel);
        }
    }

    public static void ResetAllPowerupData()
    {
        foreach (Powerup powerupName in Enum.GetValues(typeof(Powerup)))
        {
            string powerup = powerupName.ToString();
            if (PlayerPrefs.HasKey(powerup))
            {
                PlayerPrefs.SetInt(powerup, _powerupStartLevel);
            }
        }
    }

    public static void ResetInventoryData(PlayerInventory inventoryData)
    {
        string inventory = inventoryData.ToString();
        if (PlayerPrefs.HasKey(inventory))
        {
            PlayerPrefs.SetInt(inventory, 0);
        }
    }

    public static void ResetAllInventoryData()
    {
        foreach (PlayerInventory playerInventory in Enum.GetValues(typeof(PlayerInventory)))
        {
            string inventory = playerInventory.ToString();
            if (PlayerPrefs.HasKey(inventory))
            {
                PlayerPrefs.SetInt(inventory, 0);
            }
        }
    }

    public static void ResetSkinData()
    {
        foreach (SkinName skinName in Enum.GetValues(typeof(SkinName)))
        {
            string skin = skinName.ToString();
            if (PlayerPrefs.HasKey(skin))
            {
                // Zero means "not purchased"
                PlayerPrefs.SetInt(skin, 0);
            }
        }
    }

    public static void ResetAllSkins()
    {
        foreach (SkinName skinName in Enum.GetValues(typeof(SkinName)))
        {
            string skin = skinName.ToString();
            if (PlayerPrefs.HasKey(skin))
            {
                PlayerPrefs.SetInt(skin, 0);
            }
        }
    }

    public static void ResetSkinData(SkinName skinName)
    {
        string skin = skinName.ToString();
        if (PlayerPrefs.HasKey(skin))
        {
            PlayerPrefs.SetInt(skin, 0);
        }
    }
}
