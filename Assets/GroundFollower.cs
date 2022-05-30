using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFollower : MonoBehaviour
{
    Transform _player;

    [SerializeField] float zOffset;

    // Start is called before the first frame update
    void Awake()
    {
        _player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseGame.Instance.IsGamePaused && !GameOver.Instance.IsGameOver)
        {
            Vector3 positionThisFrame = new Vector3(transform.position.x, transform.position.y, _player.position.z + zOffset);

            transform.position = positionThisFrame;
        }
    }
}
