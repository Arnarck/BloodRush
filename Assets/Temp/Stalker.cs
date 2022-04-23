using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalker : MonoBehaviour
{
    bool _isActivated;

    Transform _player;

    void Awake()
    {
        _player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isActivated = true;
        }

        if (_isActivated)
        {
            Vector3 relativePosition = _player.position - transform.position;
            Quaternion lookAtPlayer = Quaternion.LookRotation(relativePosition);
            transform.rotation = lookAtPlayer;

            transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * 5f;
        }
    }
}
