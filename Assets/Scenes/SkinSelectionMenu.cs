using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SkinSelectionMenu : MonoBehaviour
{
    int _currentSkin = 0;

    public int CurrentSkin { get => _currentSkin; private set => _currentSkin = value; }

    [SerializeField] Skin[] purchasableSkins;
    [Header("Buttons")]
    [SerializeField] ButtonData confirmButton;
    [SerializeField] ButtonData purchaseButton;

    [System.Serializable]
    class Skin
    {
        public SaveData.SkinName name;
        public int price;
    }

    [System.Serializable]
    class ButtonData
    {
        public GameObject gameObject;
        public Button button;
        public TextMeshProUGUI text;
    }

    void Start()
    {
        confirmButton.gameObject.SetActive(false);
        purchaseButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SaveData.ResetAllSkins();
        }
    }

    public void SelectSkin(int index)
    {
        CurrentSkin = index;
        CurrentSkin = Mathf.Clamp(CurrentSkin, 0, SaveData.GetSkinSize());
        SetButtonState();
    }

    void SetButtonState()
    {
        SaveData.SkinName purchasableSkinName = (SaveData.SkinName)CurrentSkin - 1;
        bool isSkinPurchased = SaveData.IsSkinPurchased(purchasableSkinName);
        bool isSelectedSkin = CurrentSkin == SaveData.GetInventoryData(SaveData.PlayerInventory.SelectedSkin) ? true : false;

        if (isSelectedSkin)
        {
            SetConfirmButtonActive(true);
            SetConfirmButtonInfo("Selected", false);
        }
        else if (CurrentSkin == 0 || isSkinPurchased)
        {
            SetConfirmButtonActive(true);
            SetConfirmButtonInfo("Confirm", true);
        }
        else
        {
            SetConfirmButtonActive(false);
            purchaseButton.text.text = purchasableSkins[CurrentSkin - 1].price.ToString();
        }
    }

    void SetConfirmButtonInfo(string textMessage, bool isInteractable)
    {
        confirmButton.text.text = textMessage;
        confirmButton.button.interactable = isInteractable;
    }

    void SetConfirmButtonActive(bool isActive)
    {
        confirmButton.gameObject.SetActive(isActive);
        purchaseButton.gameObject.SetActive(!isActive);
    }

    public void SelectNextSkin()
    {
        CurrentSkin++;
        CurrentSkin = Mathf.Clamp(CurrentSkin, 0, SaveData.GetSkinSize());
    }

    public void SelectPreviousSkin()
    {
        CurrentSkin--;
        CurrentSkin = Mathf.Clamp(CurrentSkin, 0, SaveData.GetSkinSize());
    }

    public void ConfirmSelectedSkin()
    {
        SaveData.SetSelectedSkin(CurrentSkin);
        SetConfirmButtonInfo("Selected", false);
    }

    public void PurchaseSkin()
    {
        SaveData.SkinName selectedSkin = (SaveData.SkinName)CurrentSkin - 1;
        Skin skin = GetSkin(selectedSkin);
        int storedBloodAmount = SaveData.GetInventoryData(SaveData.PlayerInventory.BloodAmount);
        int buyPrice = skin.price;

        if (storedBloodAmount >= buyPrice && !SaveData.IsSkinPurchased(selectedSkin))
        {
            SaveData.ModifyBloodAmount(-buyPrice);
            SaveData.PurchaseSkin(selectedSkin);
            SetConfirmButtonActive(true);
            SetConfirmButtonInfo("Confirm", true);
        }
    }

    Skin GetSkin(SaveData.SkinName skinName)
    {
        foreach (Skin skin in purchasableSkins)
        {
            if (skin.name.Equals(skinName)) return skin;
        }

        Debug.LogError("Skin " + skinName + " not found!");
        return null;
    }
}
