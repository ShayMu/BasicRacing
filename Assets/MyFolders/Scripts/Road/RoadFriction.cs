using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class RoadFriction : MonoBehaviour {

    public CarController _car;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "road")
        {
            _car.IsOnRoad = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "road")
        {
            _car.IsOnRoad = false;
        }
    }
}
