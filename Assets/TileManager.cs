using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    List<GameObject> _activeTiles = new List<GameObject>();

    [SerializeField] Transform player;
    [SerializeField] GameObject[] tilePrefabs;
    [SerializeField] float zSpawn;
    [SerializeField] float tileLength;
    [SerializeField] int tilesOnScreen = 5;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < tilesOnScreen; i++)
        {
            if (i == 0)
            {
                SpawnTile(0);
            }
            else
            {
                SpawnTile(Random.Range(0, tilePrefabs.Length));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.z - 35f >= zSpawn - (tilesOnScreen * tileLength))
        {
            SpawnTile(Random.Range(0, tilePrefabs.Length));
            DeleteTile();
        }
    }

    void SpawnTile(int index)
    {
        GameObject tile = Instantiate(tilePrefabs[index], transform.forward * zSpawn, Quaternion.identity);
        _activeTiles.Add(tile);
        zSpawn += tileLength;
    }

    void DeleteTile()
    {
        Destroy(_activeTiles[0]);
        _activeTiles.RemoveAt(0);
    }
}
