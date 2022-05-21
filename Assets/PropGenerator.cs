using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropGenerator : MonoBehaviour
{
    enum PropType
    {
        None,
        Blood,
        Powerup,
        Obstacle
    }

    class LaneInfo
    {
        public int index = 0;
        public bool isAvaliable = true;
        public PropType propPlaced = PropType.None;
    }

    LaneInfo[] _lanes;
    PlayerController _controller;

    float _currentTimeToSpawn;
    bool _isWallRunAvaliable;

    [Header("Spawn Settings")]
    [SerializeField] float timeToSpawn = 5f;
    [SerializeField] int startPropsSpawned = 10;
    [SerializeField] float startSpawnDistance = 10f;

    [Header("Random Generation")]
    [SerializeField] int minRandom = 0;
    [SerializeField] int maxRandom = 10;

    [Header("Obstacles")]
    [SerializeField] [Range(0, 9)] int obstacleSpawnRate = 8; 
    [Tooltip("The maximum obstacle amount allowed to be spawned.")]
    [SerializeField] int maxObstacles = 2;
    [SerializeField] PoolingSystem[] obstacles;

    [Header("Blood Drops")]
    [SerializeField] int maxBlood = 3;
    [SerializeField] int bloodAmount = 7;
    [SerializeField][Range(0, 9)] int bloodSpawnRate = 8;
    [SerializeField] PoolingSystem bloodDropPrefab;

    [Header("Powerups")]
    [SerializeField] int maxPowerups = 1;
    [SerializeField][Range(0, 9)] int powerupSpawnRate = 2;
    [SerializeField] PoolingSystem[] powerupPrefabs;

    [Header("Wall Run")]
    [SerializeField] float xWallOffset = 4.5f;
    [SerializeField] float wallCooldown = 7f;
    [SerializeField][Range(0, 9)] int wallRunSpawnRate = 5;
    [SerializeField] PoolingSystem wallunPrefabs;

    public float TimeToSpawn { get => _currentTimeToSpawn; set => _currentTimeToSpawn = value; }

    void Awake()
    {
        _controller = FindObjectOfType<PlayerController>();
        _lanes = new LaneInfo[_controller.LaneAmount];
        transform.position += Vector3.forward * startSpawnDistance;
        _isWallRunAvaliable = true;
        _currentTimeToSpawn = timeToSpawn;

        for (int i = 0; i < _lanes.Length; i++)
        {
            _lanes[i] = new LaneInfo();
            _lanes[i].index = i;
        }
    }

    void Start()
    {
        for (int i = 0; i < startPropsSpawned; i++)
        {
            StartSpawnProcess();
            transform.position += Vector3.forward * startSpawnDistance;
        }

        transform.SetParent(_controller.transform);
        StartCoroutine(GenerateProps());
    }

    IEnumerator GenerateProps()
    {
        while (!GameOver.Instance.IsGameOver)
        {
            yield return new WaitForSeconds(TimeToSpawn);
            StartSpawnProcess();
        }
    }

    void StartSpawnProcess()
    {
        ResetLaneValues();
        RandomizeLanes();
        SpawnInAvaliableLanes(PropType.Obstacle);
        SpawnInAvaliableLanes(PropType.Blood);
        SpawnInAvaliableLanes(PropType.Powerup);

        if (_isWallRunAvaliable)
        {
            SpawnWallRun();
        }
    }

    void ResetLaneValues()
    {
        for (int i = 0; i < _lanes.Length; i++)
        {
            _lanes[i].isAvaliable = true;
            _lanes[i].propPlaced = PropType.None;
        }
    }

    void RandomizeLanes()
    {
        for (int i = 0; i < _lanes.Length; i++)
        {
            int randomIndex = Random.Range(0, _lanes.Length);
            LaneInfo temp = _lanes[i];

            _lanes[i] = _lanes[randomIndex];
            _lanes[randomIndex] = temp;
        }
    }

    void SpawnInAvaliableLanes(PropType prop)
    {
        int remainingProps = 0, spawnRate = 0;

        switch (prop)
        {
            case PropType.Obstacle:
                remainingProps = maxObstacles;
                spawnRate = obstacleSpawnRate;
                break;

            case PropType.Blood:
                remainingProps = maxBlood;
                spawnRate = bloodSpawnRate;
                break;

            case PropType.Powerup:
                remainingProps = maxPowerups;
                spawnRate = powerupSpawnRate;
                break;
        }

        for (int i = 0; i < _lanes.Length; i++)
        {
            bool isLaneAvaliable = _lanes[i].isAvaliable;
            bool hasRemainingProps = remainingProps > 0;
            bool canSpawn = Random.Range(minRandom, maxRandom) < spawnRate;

            if (canSpawn && isLaneAvaliable && hasRemainingProps)
            {
                remainingProps--;

                if (prop.Equals(PropType.Blood)) _lanes[i] = SpawnBloodDrops(_lanes[i]);
                else _lanes[i] = SpawnProp(_lanes[i], prop);
            }
        }
    }

    LaneInfo SpawnProp(LaneInfo lane, PropType prop)
    {
        GameObject obj = null;
        float xLanePosition = GetLanePosition(lane.index);
        Vector3 spawnPosition;

        switch (prop)
        {
            case PropType.Obstacle:
                obj = obstacles[Random.Range(0, obstacles.Length)].GetObject();
                lane.propPlaced = PropType.Obstacle;
                break;

            case PropType.Powerup:
                obj = powerupPrefabs[Random.Range(0, obstacles.Length)].GetObject();
                lane.propPlaced = PropType.Powerup;
                break;
        }

        spawnPosition = new Vector3(xLanePosition, obj.transform.position.y, transform.position.z);
        obj.transform.position = spawnPosition;
        lane.isAvaliable = false;

        return lane;
    }

    LaneInfo SpawnBloodDrops(LaneInfo lane)
    {
        GameObject[] bloodDrops = new GameObject[bloodAmount];
        Vector3 spawnPosition;
        float xLanePosition = GetLanePosition(lane.index);

        for (int i = 0; i < bloodDrops.Length; i++)
        {
            bloodDrops[i] = bloodDropPrefab.GetObject();
            spawnPosition = new Vector3(xLanePosition, bloodDrops[i].transform.position.y, transform.position.z + i);
            bloodDrops[i].transform.position = spawnPosition;
        }

        lane.propPlaced = PropType.Blood;
        lane.isAvaliable = false;

        return lane;
    }

    void SpawnWallRun()
    {
        int firstLane = 0;
        int lastLane = _controller.LaneAmount - 1;
        float xLanePosition = 0f;

        if (Random.Range(minRandom, maxRandom) < wallRunSpawnRate && _lanes[firstLane].propPlaced.Equals(PropType.Obstacle))
        {
            GameObject wall = wallunPrefabs.GetObject();
            xLanePosition = GetLanePosition(firstLane);
            wall.transform.position = new Vector3(xLanePosition - xWallOffset, wall.transform.position.y, transform.position.z);
        }
        else if (Random.Range(minRandom, maxRandom) < wallRunSpawnRate && _lanes[lastLane].propPlaced.Equals(PropType.Obstacle))
        {
            GameObject wall = wallunPrefabs.GetObject();
            xLanePosition = GetLanePosition(lastLane);
            wall.transform.position = new Vector3(xLanePosition + xWallOffset, wall.transform.position.y, transform.position.z);
        }

        StartCoroutine(CooldownToSpawnWallRun());
    }

    IEnumerator CooldownToSpawnWallRun()
    {
        _isWallRunAvaliable = false;
        yield return new WaitForSeconds(wallCooldown);
        _isWallRunAvaliable = true;
    }

    float GetLanePosition(int lane)
    {
        return (lane - _controller.MiddleLane) * _controller.LaneDistance;
    }
}
