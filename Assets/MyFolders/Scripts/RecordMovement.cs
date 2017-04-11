using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordMovement : MonoBehaviour {

    private Queue<RecordBlock> _recording;
    private float _recordingTime;
    private bool _isRecording;

	// Use this for initialization
	void Start () {
        _recording = new Queue<RecordBlock>();
        _recordingTime = 0;
        _isRecording = true;
	}
	
	// Update is called once per frame
	void Update () {

        if(_isRecording)
        {
            _recordingTime += Time.deltaTime;

            _recording.Enqueue(
                new RecordBlock()
                {
                    Position = transform.position,
                    Rotation = transform.rotation,
                    PointInTime = _recordingTime
                });
        }
	}

    public void StartRecording()
    {
        _isRecording = true;
    }

    public void StopRecording()
    {
        _isRecording = false;
    }

    public void ResetRecording()
    {
        StopRecording();
        _recordingTime = 0;
        _recording.Clear();
    }

    public Queue<RecordBlock> GetRecording()
    {
        return _recording;
    }

    public struct RecordBlock
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public float PointInTime;
    }
}
