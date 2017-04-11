using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    private static GameController _mainGameController;

    public MenuControl _menu;

    // Music support.
    public AudioClip[] _soundTrack;
    public AudioSource _audioPlayer;

    // Laps support.
    public Text[] _lapsTexts;
    public Text _totalTimeText;
    private float _totalTime;
    private int _currLap;
    private float[] _lapsTime;
    private float _currLapTime;
    private const string LAP_TIME_FORMAT = "{0:000}:{1:00.000}";

    // Game support.
    public GameObject _spawnPoint;
    public GameObject _player;
    public GameObject _ghost;
    private bool _isFinished;
    [HideInInspector]
    public bool _isGameOver;

    // High score support.
    private const string HIGH_SCORE_KEY = "BEST_TOTAL_TIME";
    private float _currHighScore;
    public Text _highScoreText;

    // Recording support.
    private const string RECORDING_FILE_PATH = "ghostRecording.json";
    private Queue<RecordMovement.RecordBlock> _recording;

	// Use this for initialization
	void Start () {
        Time.timeScale = 0;
        Initialize();
	}
	
	// Update is called once per frame
	void Update () {
        
        // laps.
        if (_currLap < _lapsTexts.Length)
        {
            _currLapTime += Time.deltaTime;
            _totalTime += Time.deltaTime;

            _lapsTexts[_currLap].text = string.Format(LAP_TIME_FORMAT, (int)(_currLapTime / 60), _currLapTime % 60);
            _totalTimeText.text = string.Format(LAP_TIME_FORMAT, (int)(_totalTime / 60), _totalTime % 60);
        }

        // ghost.
        if(_ghost != null && _recording != null)
        {
            if(_recording.Count == 0)
            {
                _ghost.SetActive(false);
            }
            else
            {
                while(_recording.Count > 0 &&
                    _recording.Peek().PointInTime < _totalTime)
                {
                    RecordMovement.RecordBlock block = _recording.Dequeue();
                    _ghost.transform.position = block.Position;
                    _ghost.transform.rotation = block.Rotation;
                }
            }
        }
    }

    /// <summary>
    /// Initialize the needed parameters for a new game.
    /// </summary>
    public void Initialize()
    {
        _isGameOver = false;
        _currLap = 0;
        _lapsTime = new float[_lapsTexts.Length];
        _currLapTime = 0;
        _totalTime = 0;
        _isFinished = false;

        _currHighScore = PlayerPrefs.GetFloat(HIGH_SCORE_KEY);
        _highScoreText.text = string.Format(LAP_TIME_FORMAT, (int)(_currHighScore / 60), _currHighScore % 60);
        _recording = LoadRecording();

        foreach (Text lapText in _lapsTexts)
        {
            lapText.text = string.Format(LAP_TIME_FORMAT, 0, 0);
        }

        _player.transform.position = _spawnPoint.transform.position;
        _player.transform.rotation = _spawnPoint.transform.rotation;
        _player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        RecordMovement recording = _player.GetComponent<RecordMovement>();
        if (recording != null)
        {
            recording.ResetRecording();
            recording.StartRecording();
        }

        StartCoroutine("PlaySoundtrack");
    }


    /// <summary>
    /// Saves the current lap time and advance to next lap.
    /// </summary>
    public void SaveLapTime()
    {
        if (_currLap < _lapsTexts.Length)
        {
            _lapsTime[_currLap] = _currLapTime;
            _currLap++;
            _currLapTime = 0;

            if (_currLap >= _lapsTexts.Length)
            {
                _isFinished = true;
                GameOver();
            }
        }
    }

    /// <summary>
    /// End the game.
    /// </summary>
    public void GameOver()
    {
        _isGameOver = true;
        if(_isFinished)
        {
            if (_currHighScore > _totalTime || _currHighScore <= 0)
            {
                PlayerPrefs.SetFloat(HIGH_SCORE_KEY, _totalTime);
                PlayerPrefs.Save();

                RecordMovement recording = _player.GetComponent<RecordMovement>();
                if (recording != null)
                {
                    recording.StopRecording();
                    SaveRecording(recording.GetRecording());
                    recording.ResetRecording();
                }
            }
        }

        _menu.PauseGame();
    }

    /// <summary>
    /// Clear the current high score + ghost recording.
    /// </summary>
    public void ResetHighScore()
    {
        PlayerPrefs.SetFloat(HIGH_SCORE_KEY, 0);

        File.Delete(RECORDING_FILE_PATH);

        _currHighScore = PlayerPrefs.GetFloat(HIGH_SCORE_KEY);
        _highScoreText.text = string.Format(LAP_TIME_FORMAT, (int)(_currHighScore / 60), _currHighScore % 60);
        _recording = null;
    }

    public static GameController MainGameController
    {
        get
        {
            if (_mainGameController == null)
            {
                GameObject controller = GameObject.FindGameObjectWithTag("GameController");
                if (controller != null)
                {
                    _mainGameController = controller.GetComponent<GameController>();
                }
            }

            return _mainGameController;
        }
    }

    private IEnumerator PlaySoundtrack()
    {
        int currClipIdx = 0;

        while(true)
        {
            if(_audioPlayer.isPlaying == false)
            {
                _audioPlayer.clip = _soundTrack[currClipIdx++];
                _audioPlayer.time = 0;
                _audioPlayer.Play();

                if (currClipIdx >= _soundTrack.Length) currClipIdx = 0;
            }
            yield return new WaitForSeconds(1);
        }
    }

    /// <summary>
    /// Saves the player recording if he broke the record.
    /// </summary>
    private void SaveRecording(Queue<RecordMovement.RecordBlock> recording)
    {
        GhostJson jsonSave = new GhostJson() { points = new RecordingJson[recording.Count] };
        int idx = 0;
        while (recording.Count != 0)
        {
            RecordMovement.RecordBlock currBlock = recording.Dequeue();
            jsonSave.points[idx] = new RecordingJson()
            {
                pos = new ThreeNumbersSave(currBlock.Position.x, currBlock.Position.y, currBlock.Position.z),
                rotation = new FourNumbersSave(currBlock.Rotation.x, currBlock.Rotation.y, currBlock.Rotation.z, currBlock.Rotation.w),
                time = currBlock.PointInTime
            };

            idx++;
        }

        string jsonString = JsonUtility.ToJson(jsonSave);

        File.WriteAllText(RECORDING_FILE_PATH, jsonString);
    }

    /// <summary>
    /// Load a ghost recording.
    /// </summary>
    private Queue<RecordMovement.RecordBlock> LoadRecording()
    {
        if (File.Exists(RECORDING_FILE_PATH) == false) return null;

        string jsonString = File.ReadAllText(RECORDING_FILE_PATH);
        GhostJson jsonSave = JsonUtility.FromJson<GhostJson>(jsonString);

        Queue<RecordMovement.RecordBlock> recording = new Queue<RecordMovement.RecordBlock>();

        for (int i = 0; i < jsonSave.points.Length; i++)
        {
            recording.Enqueue(new RecordMovement.RecordBlock()
            {
                PointInTime = jsonSave.points[i].time,
                Position = jsonSave.points[i].pos.ToVector3(),
                Rotation = jsonSave.points[i].rotation.ToQuat()
            });
        }

        return recording;
    }

    [Serializable]
    private struct GhostJson
    {
        public RecordingJson[] points;
    }

    [Serializable]
    private struct RecordingJson
    {
        public ThreeNumbersSave pos;
        public FourNumbersSave rotation;
        public float time;
    }

    [Serializable]
    private struct ThreeNumbersSave
    {
        public float x;
        public float y;
        public float z;

        public ThreeNumbersSave(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    private struct FourNumbersSave
    {
        public ThreeNumbersSave xyz;
        public float w;

        public FourNumbersSave(float x, float y, float z, float w)
        {
            xyz = new ThreeNumbersSave(x, y, z);
            this.w = w;
        }

        public Quaternion ToQuat()
        {
            return new Quaternion(xyz.x, xyz.y, xyz.z, w);
        }
    }
}
