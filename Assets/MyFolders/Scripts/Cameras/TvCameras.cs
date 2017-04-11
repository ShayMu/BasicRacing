using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TvCameras : MonoBehaviour {

    private const int ACTIVE_DEPTH = 1;
    private const int NOT_ACTIVE_DEPTH = -10;

    public GameObject _tvCamerasParent;

    private bool _showingTvCameras;
    private List<Camera> _tvCameras;
    private int _currCameraIdx;

	// Use this for initialization
	void Start () {

        _tvCameras = new List<Camera>();
        _currCameraIdx = 0;

		foreach(Transform tvCam in _tvCamerasParent.transform)
        {
            Camera cam = tvCam.GetComponent<Camera>();
            if(cam != null)
            {
                cam.depth = NOT_ACTIVE_DEPTH;
                _tvCameras.Add(cam);
            }
        }

        InvokeRepeating("CameraChange", 0, 4);

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("CameraSwitch"))
        {
            _showingTvCameras = !_showingTvCameras;
            CameraChange();
        }

	}

    private void CameraChange()
    {
        _tvCameras[_currCameraIdx].depth = NOT_ACTIVE_DEPTH;

        _currCameraIdx++;
        if(_currCameraIdx >= _tvCameras.Count)
        {
            _currCameraIdx = 0;
        }

        _tvCameras[_currCameraIdx].depth = _showingTvCameras ? ACTIVE_DEPTH : NOT_ACTIVE_DEPTH;
    }
}
