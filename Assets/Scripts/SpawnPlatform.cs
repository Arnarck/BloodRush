using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatform : MonoBehaviour
{
    public List<GameObject> platforms = new List<GameObject>();
    public List<Transform> currentPlatforms = new List<Transform>();
    private int _offset= 0;
    private int _platformIndex;
    private Transform _player;
    private GameObject _playerGameObject;
    private Transform _currentPlatformPoint;


    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerGameObject = GameObject.FindGameObjectWithTag("Player");

        for (int i = 0; i < platforms.Count; i++)
        {
            Transform p = Instantiate(platforms[i], new Vector3(0,0,i*122), transform.rotation).transform;
            currentPlatforms.Add(p);
            _offset += 122;
        }
        _currentPlatformPoint = currentPlatforms[_platformIndex].GetComponent<Platform>().point;
    }


    void Update()
    {
        float distance = _player.position.z - _currentPlatformPoint.position.z; 
        if (distance >=5)
        {
            Recycle(currentPlatforms[_platformIndex].gameObject);
            _platformIndex++;
            if (_platformIndex > currentPlatforms.Count-1)
            {
                _platformIndex = 0;
            }
            _currentPlatformPoint = currentPlatforms[_platformIndex].GetComponent<Platform>().point;
        }
    }

    public void Recycle(GameObject platform)
    {
        platform.transform.position = new Vector3(0,0,_offset);
        _offset += 122;
    }
}
