using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    [SerializeField] Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        if (SaveData.IsSkinPurchased(SaveData.SkinName.MetalDemiAdany))
        {
            GameObject.FindWithTag("Player").transform.GetChild(0).GetComponent<MeshFilter>().mesh = mesh;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
