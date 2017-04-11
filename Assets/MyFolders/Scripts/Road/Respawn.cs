using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class Respawn : MonoBehaviour {

    public GameObject _playerObj;
    private List<Transform> _respawnPoints;

	// Use this for initialization
	void Start () {
        _respawnPoints = new List<Transform>();

		foreach(Transform respawn in transform)
        {
            _respawnPoints.Add(respawn);
        }

        _respawnPoints = _respawnPoints.OrderBy(item => item.name).ToList();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Respawn"))
        {
            if(_playerObj.GetComponent<CarController>().IsOnRoad == false)
            {
                RespawnAtNearestPlace();
            }
        }
	}

    private void RespawnAtNearestPlace()
    {
        Transform respawnPoint = _respawnPoints[0];
        float respawnDistance = Vector3.Distance(_playerObj.transform.position, _respawnPoints[0].position);

        for (int i = 1; i < _respawnPoints.Count; i++)
        {
            float distance = Vector3.Distance(_playerObj.transform.position, _respawnPoints[i].position);

            if(distance < respawnDistance)
            {
                respawnPoint = _respawnPoints[i];
                respawnDistance = distance;
            }
        }

        if(_playerObj != null)
        {
            Vector3 newPos = respawnPoint.position;
            newPos.y += 5;
            _playerObj.transform.position = newPos;

            Quaternion newRotation = Quaternion.Euler(0, -90, 0);
            newRotation.x += respawnPoint.rotation.x;
            newRotation.y += respawnPoint.rotation.y;
            newRotation.z += respawnPoint.rotation.z;
            _playerObj.transform.rotation = respawnPoint.rotation;
            _playerObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
