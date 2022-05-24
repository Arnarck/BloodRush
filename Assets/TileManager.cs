using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] PoolingSystem[] tilePrefabs;
    [SerializeField] float zSpawn;
    [SerializeField] float tileLength;
    [SerializeField] int tilesOnScreen = 5;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < tilesOnScreen; i++)
        {
            SpawnTile();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.z - 35f >= zSpawn - (tilesOnScreen * tileLength))
        {
            SpawnTile();
        }
    }

    void SpawnTile()
    {
        GameObject tile = tilePrefabs[Random.Range(0, tilePrefabs.Length)].GetObject();
        tile.transform.position = transform.forward * zSpawn;
        zSpawn += tileLength;
    }
}
