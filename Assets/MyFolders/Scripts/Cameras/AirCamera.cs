﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCamera : MonoBehaviour {

    public GameObject _pointToFocus;
    public float _maxDistance;

	// Use this for initialization
	void Start () {
        ChangeCameraPosition();
    }
	
	// Update is called once per frame
	void Update () {

        if(Vector3.Distance(transform.position, _pointToFocus.transform.position) > (_maxDistance * 3))
        {
            ChangeCameraPosition();
        }

        transform.LookAt(_pointToFocus.transform.position);
	}

    private void ChangeCameraPosition()
    {
        Vector3 newPos = _pointToFocus.transform.position;
        newPos += (_pointToFocus.transform.up + _pointToFocus.transform.forward + _pointToFocus.transform.right) * _maxDistance;
        transform.position = newPos;
    }
}
