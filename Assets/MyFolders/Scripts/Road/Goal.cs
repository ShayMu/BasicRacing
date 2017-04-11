using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    public Checkpoint[] _lapCheckpoints;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(IsPassedAllCheckpoints())
        {
            if (other.tag == "Player")
            {
                GameController.MainGameController.SaveLapTime();
                ResetCheckpoints();
            }
        }
    }

    private bool IsPassedAllCheckpoints()
    {
        bool isPassedAll = true;

        foreach(Checkpoint point in _lapCheckpoints)
        {
            if(point.IsPlayerPassed == false)
            {
                isPassedAll = false;
                break;
            }
        }

        return isPassedAll;
    }

    private void ResetCheckpoints()
    {
        foreach(Checkpoint point in _lapCheckpoints)
        {
            point.IsPlayerPassed = false;
        }
    }
}
