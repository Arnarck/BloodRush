using UnityEngine;
using System;

public class SkinSelection : MonoBehaviour
{

    [SerializeField] SkinnedMeshRenderer _playerMesh;
    [SerializeField] Mesh demiAdanySkin;
    [SerializeField] SkinData[] avaliableSkins;

    [Serializable]
    class SkinData
    {
        public SaveData.SkinName name;
        public Mesh mesh;
    }

    // Start is called before the first frame update
    void Start()
    {
        int selectedSkinIndex = SaveData.GetInventoryData(SaveData.PlayerInventory.SelectedSkin);

        if (selectedSkinIndex == 0)
        {
            _playerMesh.sharedMesh = demiAdanySkin;
            return;
        }

        foreach (SkinData skin in avaliableSkins)
        {
            SaveData.SkinName selectedSkin = (SaveData.SkinName)selectedSkinIndex - 1;
            if (skin.name.Equals(selectedSkin))
            {
                Debug.Log(selectedSkin.ToString());
                _playerMesh.sharedMesh = skin.mesh;
            }
        }
    }
}
