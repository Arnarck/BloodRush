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

    [SerializeField] GameObject[] obstacles;
    [Tooltip("The maximum obstacle amount allowed to be spawned.")]
    [SerializeField] int maxObstacles = 2;

    void Awake()
    {
        _controller = FindObjectOfType<PlayerController>();
        _lanes = new LaneInfo[_controller.LaneAmount];

        for (int i = 0; i < _lanes.Length; i++)
        {
            _lanes[i] = new LaneInfo();
            _lanes[i].index = i;
        }
    }

    void Start()
    {
        int remainingObstacles = maxObstacles;

        for (int i = 0; i < _lanes.Length; i++)
        {
            if (_lanes[i].isAvaliable && remainingObstacles > 0)
            {
                remainingObstacles--;
                _lanes[i] = SpawnObstacle(_lanes[i]);
            }
        }
    }

    LaneInfo SpawnObstacle(LaneInfo lane)
    {
        float xLanePosition = GetLanePosition(lane.index);
        Vector3 spawnPosition = new Vector3(xLanePosition, transform.position.y, transform.position.z);

        Instantiate(obstacles[Random.Range(0, obstacles.Length)], spawnPosition, Quaternion.identity);

        lane.isAvaliable = false;
        lane.propPlaced = PropType.Obstacle;

        return lane;
    }

    float GetLanePosition(int lane)
    {
        return (lane - _controller.MiddleLane) * _controller.LaneDistance;
    }
}
