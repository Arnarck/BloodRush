using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointCounter : MonoBehaviour
{

    public GameObject text;
    public int count = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        count++;
        text.GetComponent<Text>().text = count.ToString();
        if (count>100000 || Input.GetKeyDown(KeyCode.P))
        {
            RenderSettings.fogColor = Color.white;
            RenderSettings.fogDensity = 0.01f;          
        }
    }
}
